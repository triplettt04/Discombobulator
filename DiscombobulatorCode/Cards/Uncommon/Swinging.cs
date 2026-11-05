using Discombobulator.DiscombobulatorCode.Cards;
using Discombobulator.DiscombobulatorCode.Modifications;
using Discombobulator.DiscombobulatorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Discombobulator.DiscombobulatorCode.Cards.Uncommon;

public class Swinging() : DiscombobulatorCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<SwingPower>(),
        HoverTipFactory.FromKeyword(DiscombobulatorKeywords.Transfer)
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(1), new PowerVar<SwingPower>(2)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            this.Owner.Creature, 
            "PowerUp", 
            this.Owner.Character.PowerUpAnimDelay);
        
        await PowerCmd.Apply<StrengthPower>(
            choiceContext, 
            Owner.Creature, 
            DynamicVars["StrengthPower"].BaseValue, 
            Owner.Creature, 
            this);

        await PowerCmd.Apply<SwingPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[SwingPower.Key].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[SwingPower.Key].UpgradeValueBy(1);
    }
}