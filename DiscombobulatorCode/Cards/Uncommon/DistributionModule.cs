using Discombobulator.DiscombobulatorCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Discombobulator.DiscombobulatorCode.Cards.Uncommon;


public class DistributionModule() : DiscombobulatorCard(2,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(2)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<StrengthPower>(
            choiceContext, 
            Owner.Creature, 
            DynamicVars["StrengthPower"].BaseValue, 
            Owner.Creature, 
            this);
    }

    protected override void OnUpgrade() => DynamicVars["StrengthPower"].UpgradeValueBy(1);
}