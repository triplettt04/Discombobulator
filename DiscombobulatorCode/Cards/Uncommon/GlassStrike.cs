using Discombobulator.DiscombobulatorCode.Cards;
using Discombobulator.DiscombobulatorCode.Modifications;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Discombobulator.DiscombobulatorCode.Cards.Uncommon;


public class GlassStrike() : DiscombobulatorCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new TransferVar(7)];

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        AttackCommand attack = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        await TransferCmd.Transfer(
            this,
            attack._calculatedDamageVar == null ? attack._damagePerHit : attack._calculatedDamageVar.Calculate(play.Target),
            DamageVar.defaultName,
            choiceContext,
            Owner,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}