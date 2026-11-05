using Discombobulator.DiscombobulatorCode.Cards;
using Discombobulator.DiscombobulatorCode.Modifications;
using Discombobulator.DiscombobulatorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Discombobulator.DiscombobulatorCode.Powers;

public class StepPower() : DiscombobulatorPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public static string Key => "StepPower";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DiscombobulatorKeywords.Transfer),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        
        await TransferCmd.Transfer(
            null,
            Amount,
            false,
            choiceContext,
            player,
            this);
    }
}