using Discombobulator.DiscombobulatorCode.Cards;
using Discombobulator.DiscombobulatorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Discombobulator.DiscombobulatorCode.Cards.Uncommon;

public class Stepping() : DiscombobulatorCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<StepPower>(),
        HoverTipFactory.FromKeyword(DiscombobulatorKeywords.Transfer)
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>(1), new PowerVar<StepPower>(2)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            this.Owner.Creature, 
            "PowerUp", 
            this.Owner.Character.PowerUpAnimDelay);
        
        await PowerCmd.Apply<DexterityPower>(
            choiceContext, 
            Owner.Creature, 
            DynamicVars["DexterityPower"].BaseValue, 
            Owner.Creature, 
            this);

        await PowerCmd.Apply<StepPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[StepPower.Key].BaseValue,
            Owner.Creature,
            this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars[StepPower.Key].UpgradeValueBy(1);
    }
}