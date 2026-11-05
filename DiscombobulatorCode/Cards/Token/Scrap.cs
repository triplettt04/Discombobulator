using Discombobulator.DiscombobulatorCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Discombobulator.DiscombobulatorCode.Cards.Token;

public class Scrap() : DiscombobulatorCard(1,
    CardType.Attack, CardRarity.Token,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(0, ValueProp.Move), new BlockVar(0, ValueProp.Move)];

    public override bool GainsBlock => true;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState,
        Player? creator = null)
    {
        if (count == 0)
            return Array.Empty<CardModel>();
        
        if (CombatManager.Instance.IsOverOrEnding)
            return Array.Empty<CardModel>();
        
        List<CardModel> scraps = [];
        
        for (int index = 0; index < count; ++index)
            scraps.Add(combatState.CreateCard<Scrap>(owner));
        
        await CardPileCmd.AddGeneratedCardsToCombat(scraps, PileType.Hand, creator ?? owner);
        
        return scraps;
    }
}