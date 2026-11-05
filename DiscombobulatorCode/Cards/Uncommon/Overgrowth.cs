using Discombobulator.DiscombobulatorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Discombobulator.DiscombobulatorCode.Cards.Uncommon;

public class Overgrowth() : DiscombobulatorCard(2,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<OvergrowthPower>(1)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<OvergrowthPower>(
            choiceContext, 
            Owner.Creature, 
            DynamicVars[OvergrowthPower.Name].BaseValue, 
            Owner.Creature, 
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[OvergrowthPower.Name].UpgradeValueBy(1);
    }
}