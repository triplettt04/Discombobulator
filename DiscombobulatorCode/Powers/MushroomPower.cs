using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;

namespace Discombobulator.DiscombobulatorCode.Powers;

public class MushroomPower() : DiscombobulatorPower
{
    public static string Name => "MushroomPower";
    
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    //PowerModel, Amount, scalesWithMultiplayer
    public static IEnumerable<(Type, Decimal, bool)> SupportedPowers =>
    [
        (typeof(StrengthPower), -1, false),
        (typeof(DexterityPower), -3, true),
        (typeof(DemisePower), 2, true),
        (typeof(DoomPower), 10, true),
        (typeof(PoisonPower), 4, true),
        (typeof(ShrinkPower), 1, false),
        (typeof(VulnerablePower), 1, false),
        (typeof(WeakPower), 1, false),
        (typeof(ManglePower), 3, false),
        (typeof(DebilitatePower), 1, false),
        (typeof(FrailPower), 2, false), 
        (typeof(TaintedPower), 3, true), 
        (typeof(StranglePower), 2, true), 
    ];

    // public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    // {
    //     if (!participants.Contains(Owner) || side == CombatSide.Player)
    //         return;
    //     
    //     Run(participants, CombatState);
    // }
    //
    // public override async Task AfterSideTurnStart(
    //     CombatSide side,
    //     IReadOnlyList<Creature> participants,
    //     ICombatState combatState)
    // {
    //     if (!participants.Contains(Owner) || side == CombatSide.Enemy)
    //         return;
    //     
    //     Run(participants, combatState);
    // }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await Run(CombatState);
    }

    private async Task Run(ICombatState combatState)
    {
        int nextRng = combatState.RunState.Rng.Niche.NextInt(SupportedPowers.Count());
        
        (Type powerType, Decimal scale, bool scalesWithMultiplayer) = SupportedPowers
            .ElementAt(nextRng);
        
        if (!typeof(PowerModel).IsAssignableFrom(powerType) || powerType.IsAbstract)
            throw new ArgumentException("Not a concrete PowerModel type.", nameof(powerType));
        
        //Build the generic method for Apply<T>(...) for whichever power is selected
        var signature = new[]
        {
            typeof(PlayerChoiceContext),
            typeof(Creature),
            typeof(Decimal),
            typeof(Creature),
            typeof(CardModel),
            typeof(bool)
        };
        
        MethodInfo applyDefinition = typeof(PowerCmd)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method =>
                method.Name == nameof(PowerCmd.Apply) &&
                method.IsGenericMethodDefinition &&
                method.GetGenericArguments().Length == 1 &&
                method.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .SequenceEqual(signature));
        
        MethodInfo closedApply = applyDefinition.MakeGenericMethod(powerType);
        Decimal multiplier = combatState.Players.Count();
        
        var task = (Task)closedApply.Invoke(
            null,
            new object?[]
            {
                new ThrowingPlayerChoiceContext(),
                Owner,
                scalesWithMultiplayer ? scale * multiplier : scale,
                Applier,
                null,
                false
            })!;

        await task;

        if (Owner.IsAlive)
            await PowerCmd.Decrement(this);
        else
            await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
}