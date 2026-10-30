using BaseLib.Extensions;
using Discombobulator.DiscombobulatorCode.Cards;
using Discombobulator.DiscombobulatorCode.Modifications;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Discombobulator.DiscombobulatorCode.Extensions;
using MegaCrit.Sts2.Core.HoverTips;

namespace Discombobulator.DiscombobulatorCode.Cards.Basic;

public class Combine() : DiscombobulatorCard(0,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new TransferVar(4)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        //Choose two attacks to transfer
        IEnumerable<CardModel> cardModel = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(
                CardSelectorPrefsExtension.TransferSelectionPrompt,
                2),
            TransferCmd.CanTransferDamage,
            this);
        
        //Transfer this.damage from choice 1 -> choice 2
        if (cardModel != null && cardModel.Count() >= 2)
        {
            await TransferCmd.Transfer(
                cardModel.ElementAt(0), 
                cardModel.Skip(1), 
                DynamicVars[TransferVar.Key].BaseValue,
                DamageVar.defaultName);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[TransferVar.Key].BaseValue += 2;
    }
}