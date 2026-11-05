using Discombobulator.DiscombobulatorCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Discombobulator.DiscombobulatorCode.Cards.Common;

public class Scrounge() : DiscombobulatorCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Scrap>()];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars[CardsVar.defaultName].BaseValue, Owner);
        await Scrap.CreateInHand(Owner, 1, CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[CardsVar.defaultName].UpgradeValueBy(1);
    }
}