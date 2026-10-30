using Discombobulator.DiscombobulatorCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Discombobulator.DiscombobulatorCode.Cards.Common;

public class Discombobulate() : DiscombobulatorCard(1,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        //Decimal num = await CreatureCmd.
    }

    protected override void OnUpgrade()
    {

    }
}