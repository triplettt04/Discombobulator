using Discombobulator.DiscombobulatorCode.Modifications;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Discombobulator.DiscombobulatorCode.Extensions;

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
        await TransferCmd.Transfer(
            null,
            DynamicVars[TransferVar.Key].BaseValue,
            DamageVar.defaultName,
            choiceContext,
            Owner,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[TransferVar.Key].UpgradeValueBy(2);
    }
}