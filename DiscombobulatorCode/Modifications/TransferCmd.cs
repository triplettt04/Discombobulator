using BaseLib.Abstracts;
using Discombobulator.DiscombobulatorCode.Extensions;
using Discombobulator.DiscombobulatorCode.Modifications;
using Discombobulator.DiscombobulatorCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
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
    public static bool CanTransferDamage(CardModel cardModel) => cardModel.Type == CardType.Attack;
    
    public static bool CanTransferBlock(CardModel cardModel) => cardModel.GainsBlock;
    
    public static async Task Transfer(
        CardModel? giver,
        decimal transferAmount,
        bool isDamage,
        PlayerChoiceContext choiceContext,
        Player player,
        AbstractModel source
    )
    {
        IEnumerable<CardModel> targets;
        IEnumerable<CardModel> cards = await SelectFromHand(
            choiceContext,
            player,
            new CardSelectorPrefs(
                CardSelectorPrefsExtension.TransferSelectionPrompt(source, isDamage),
                giver == null ? 2 : 1),
            isDamage ? CanTransferDamage : CanTransferBlock,
            source);
        
        if(cards.Count() == 0)
            return;
        
        if (giver == null)
        {
            giver = cards.FirstOrDefault();
            targets = cards.Skip(1);
        }
        else
        {
            targets = cards;
        }
        
        List<string> varNames = CardMethods.VarNames(giver, isDamage);
        foreach(var x in varNames)
            Log.Info("possible varName: " + x);
        if (varNames.Count() == 0)
            return;
        
        string varName = varNames.First();
        decimal amount = Math.Min(giver.DynamicVars[varName].BaseValue, transferAmount);

        LogTransfer(source, amount, isDamage);
        giver.DynamicVars[varName].BaseValue -= amount;
        
        foreach (CardModel target in targets)
        {
            var targetName = CardMethods.VarNames(target, isDamage).FirstOrDefault();
            if (targetName == null)
                return;
            target.DynamicVars[targetName].BaseValue += amount;
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
            return Array.Empty<CardModel>();
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
            result = list;
        else if (!prefs.RequireManualConfirmation && list.Count < prefs.MinSelect)
            result = list;
        else if (CardSelectCmd.ShouldSelectLocalCard(player))
        {
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
            
        CardSelectCmd.LogChoice(player, result);
        return result;
    }

    public static IEnumerable<(decimal transfer, bool isDamage)> LogTransfer(
        AbstractModel model,
        decimal transfer,
        bool isDamage
    )
    {
        if (model is CardModel)
        {
            if(!((CardModel)model).DynamicVars.TryGetValue(TransferVar.Key, out DynamicVar value))
                return [];
            
            TransferVar transferVar = (TransferVar)value;
            transferVar.ActualTransfers.Add(new (transfer, isDamage));

            return transferVar.ActualTransfers;
        }
        
        if (model is DiscombobulatorPower)
        {
            DiscombobulatorPower transferPower = (DiscombobulatorPower)model;
            transferPower.ActualTransfers.Add(new (transfer, isDamage));

            return transferPower.ActualTransfers;
        }
            
        throw new NotSupportedException("Tried transferring from a non-card and non-power");
    }
    
    public static decimal TotalTransfers(
        CardModel cardModel,
        Creature? creature,
        bool? isDamage)
    {
        IEnumerable<CardPlayFinishedEntry> transferEntries = CombatManager.Instance.History.CardPlaysFinished.Where(entry =>
            entry.CardPlay.Card.DynamicVars.ContainsKey(TransferVar.Key));

        decimal transfersFromCards = transferEntries
            .Sum(entry => ((TransferVar)entry.CardPlay.Card.DynamicVars[TransferVar.Key]).ActualTransfers
                .Where(x => isDamage == null ? true : x.isDamage == isDamage)
                .Sum(x => x.transfer));

        IEnumerable<PowerModel> transferPowers = cardModel.Owner.Creature.Powers
            .Where(p => p is DiscombobulatorPower);
        
        decimal transfersFromPowers = transferPowers
            .Sum(p => ((DiscombobulatorPower)p).ActualTransfers
                .Where(x => isDamage == null ? true : x.isDamage==isDamage)
                .Sum(x => x.transfer));
        
        return transfersFromCards + transfersFromPowers;
    }
}