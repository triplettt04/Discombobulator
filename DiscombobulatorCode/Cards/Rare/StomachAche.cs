using Discombobulator.DiscombobulatorCode.Cards;
using Discombobulator.DiscombobulatorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Discombobulator.DiscombobulatorCode.Cards.Rare;

public class StomachAche() : DiscombobulatorCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MushroomPower>(3)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<MushroomPower>(
            choiceContext, 
            Owner.Creature, 
            1, 
            Owner.Creature, 
            this);
        
        await PowerCmd.Apply<MushroomPower>(
            choiceContext, 
            CombatState?.HittableEnemies, 
            DynamicVars[MushroomPower.Name].BaseValue, 
            Owner.Creature, 
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[MushroomPower.Name].UpgradeValueBy(1);
    }
}