using Discombobulator.DiscombobulatorCode.Modifications;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;

namespace Discombobulator.DiscombobulatorCode.Cards.Basic;

public class Combine() : DiscombobulatorCard(0,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [DiscombobulatorKeywords.Transfer];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new TransferVar(4)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        //Choose two attacks to transfer
        await TransferCmd.Transfer(
            null,
            DynamicVars[TransferVar.Key].BaseValue,
            true,
            choiceContext,
            Owner,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[TransferVar.Key].UpgradeValueBy(2);
    }
}