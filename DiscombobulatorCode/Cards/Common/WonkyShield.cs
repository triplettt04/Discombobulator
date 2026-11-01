using Discombobulator.DiscombobulatorCode.Cards;
using Discombobulator.DiscombobulatorCode.Modifications;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Discombobulator.DiscombobulatorCode.Cards.Common;

public class WonkyShield() : DiscombobulatorCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4, ValueProp.Move), new TransferVar(2)];
    
    public override bool GainsBlock => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play, true);
        await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play);
        
        await TransferCmd.Transfer(
            this,
            DynamicVars[TransferVar.Key].BaseValue,
            BlockVar.defaultName,
            choiceContext,
            Owner,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}