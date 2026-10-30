using Discombobulator.DiscombobulatorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Discombobulator.DiscombobulatorCode.Powers;

public class Distribution() : DiscombobulatorPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override bool AllowNegative => true;

    public override Decimal ModifyPowerAmountGivenAdditive(
        PowerModel power, 
        Creature giver, 
        decimal amount, 
        Creature? target,
        CardModel? cardSource)
    {
        return this.Amount;
    }
}