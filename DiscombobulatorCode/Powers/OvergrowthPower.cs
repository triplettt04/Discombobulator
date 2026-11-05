using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Discombobulator.DiscombobulatorCode.Powers;

public class OvergrowthPower() : DiscombobulatorPower
{
    public static string Name => "OvergrowthPower";
    
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MushroomPower>()];

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return;
        
        Flash();
        Creature target;

        for (var i = 0; i < Amount; i++)
        {
            target = Owner.Player.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);

            if (target == null)
                return;

            await PowerCmd.Apply<MushroomPower>(
                new ThrowingPlayerChoiceContext(),
                target,
                1,
                Owner,
                null);
        }
    }
}