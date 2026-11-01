using BaseLib.Abstracts;
using Discombobulator.DiscombobulatorCode.Cards;
using Discombobulator.DiscombobulatorCode.Extensions;
using Discombobulator.DiscombobulatorCode.Modifications;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Discombobulator.DiscombobulatorCode.Cards.Common;

public class WonkySpear() : DiscombobulatorCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new TransferVar(2)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd
            .Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitCount(2)
            .FromCard((CardModel) this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        await TransferCmd.Transfer(
            this,
            DynamicVars[TransferVar.Key].BaseValue,
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