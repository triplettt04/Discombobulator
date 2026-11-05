using Discombobulator.DiscombobulatorCode.Cards;
using Discombobulator.DiscombobulatorCode.Modifications;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Discombobulator.DiscombobulatorCode.Cards.Uncommon;

public class ScrapeScraps() : DiscombobulatorCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new CardsVar(4)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        await CardCmd.Discard(
            choiceContext, 
            (await CardPileCmd.Draw(choiceContext, DynamicVars[CardsVar.defaultName].BaseValue, Owner))
                .Where(c => !CardMethods.HasZeroAttackOrBlock(c)));
    }

    protected override void OnUpgrade()
    {
        DynamicVars[CardsVar.defaultName].UpgradeValueBy(1);
    }
}