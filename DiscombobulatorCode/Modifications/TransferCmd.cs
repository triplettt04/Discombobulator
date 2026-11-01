using BaseLib.Abstracts;
using Discombobulator.DiscombobulatorCode.Extensions;
using Discombobulator.DiscombobulatorCode.Modifications;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Discombobulator.DiscombobulatorCode.Cards;

public class TransferCmd
{
    public static async Task Transfer(
        CardModel? giver,
        decimal transferAmount,
        string varName,
        PlayerChoiceContext choiceContext,
        Player player,
        AbstractModel source
    )
    {
        IEnumerable<CardModel> targets;
        if (giver == null)
        {
            IEnumerable<CardModel> cards = await SelectFromHand(
                choiceContext,
                player,
                new CardSelectorPrefs(
                    CardSelectorPrefsExtension.TransferDamageSelectionPrompt,
                    2),
                varName == DamageVar.defaultName ? CanTransferDamage : CanTransferBlock,
                source);

            if (!cards.Any())
                return;

            giver = cards.FirstOrDefault();
            targets = cards.Skip(1);
        }
        else
        {
            targets = await SelectFromHand(
                choiceContext,
                player,
                new CardSelectorPrefs(
                    CardSelectorPrefsExtension.TransferDamageSelectionPrompt,
                    1),
                varName == DamageVar.defaultName ? CanTransferDamage : CanTransferBlock,
                source);
            
            if (!targets.Any())
                return;
        }
        
        decimal amount = Math.Min(giver.DynamicVars[varName].BaseValue, transferAmount);
        
        giver.DynamicVars[varName].BaseValue -= amount;
        
        foreach (CardModel target in targets)
        {
            target.DynamicVars[varName].BaseValue += amount;
        }
        
        NCard.FindOnTable(giver)?.ReloadOverlay();
        foreach (CardModel target in targets)
        {
            NCard.FindOnTable(target)?.ReloadOverlay();
        }
    }

    public static async Task<IEnumerable<CardModel>> SelectFromHand(
        PlayerChoiceContext context,
        Player player,
        CardSelectorPrefs prefs,
        Func<CardModel, bool>? filter,
        AbstractModel source
    )
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        if (CardSelectCmd.ShouldSelectLocalCard(player))
            NPlayerHand.Instance?.CancelAllCardPlay();
        CardSelectCmd.UndoEndTurnIfNecessary(player);
        uint? choiceId = new uint?();
        if (CardSelectCmd.Selector == null)
        {
            choiceId = new uint?(RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player));
            await context.SignalPlayerChoiceBegun(player, PlayerChoiceOptions.CancelPlayCardActions);
        }
        List<CardModel> list = PileType.Hand.GetPile(player).Cards.Where<CardModel>(filter ?? (Func<CardModel, bool>) (_ => true)).ToList<CardModel>();
        IEnumerable<CardModel> result;
        if (list.Count == 0)
            result = (IEnumerable<CardModel>) list;
        else if (CardSelectCmd.ShouldSelectLocalCard(player))
        {
            //TODO: Come back here to the following function for adjusting card selection preview / look
            //See if we can preview the card here
            result = await NCombatRoom.Instance.Ui.Hand.SelectCards(prefs, filter, source);
            RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId.Value, PlayerChoiceResult.FromMutableCombatCards(result));
        }
        else
        {
            result = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId.Value))
                .AsCombatCards();
        }

        if (choiceId.HasValue)
            await context.SignalPlayerChoiceEnded();
            
        MainFile.Logger.Info("Log Choice");
        CardSelectCmd.LogChoice(player, result);
        return result;
    }

    public static bool CanTransferDamage(CardModel cardModel)
    {
        //Determine if the card is an attack
        return cardModel.Type == CardType.Attack; // && cardModel.DynamicVars.Damage.BaseValue > 0
    }
    
    public static bool CanTransferBlock(CardModel cardModel)
    {
        //Determine if the card gains block
        return cardModel.GainsBlock;
    }
}