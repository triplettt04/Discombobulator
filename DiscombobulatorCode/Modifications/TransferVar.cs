using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Discombobulator.DiscombobulatorCode.Modifications;

public class TransferVar : DynamicVar
{
    public const string Key = "Transfer";

    public TransferVar(decimal transferAmount) : base("Transfer", transferAmount)
    {
        this.WithTooltip();
    }

    /*public static IEnumerable<decimal> TransferAmount(
        CardModel giver,
        IEnumerable<CardModel> targets,
        decimal transferAmount,
        bool isAttack
    )
    {
        string type = isAttack ? "Block" : "Damage";
        decimal amount = Math.Min(giver.DynamicVars[type].BaseValue, transferAmount);
        
        giver.DynamicVars[type].BaseValue -= amount;
        IEnumerable<decimal> previews = new[] {giver.DynamicVars[type].BaseValue};
        
        foreach (CardModel target in targets)
        {
            target.DynamicVars[type].BaseValue += amount;
        }

        return [giver.DynamicVars[type].BaseValue, ];
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        this.PreviewValue = (Decimal) .UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
    */
}