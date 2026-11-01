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

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        //Test
        //this.PreviewValue = 17;
    }
}