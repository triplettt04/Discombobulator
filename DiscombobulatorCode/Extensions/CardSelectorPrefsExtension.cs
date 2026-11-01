using MegaCrit.Sts2.Core.Localization;

namespace Discombobulator.DiscombobulatorCode.Extensions;

public static class CardSelectorPrefsExtension
{
    public static LocString TransferDamageSelectionPrompt => new LocString("card_selection", "TO_TRANSFER_DAMAGE");
    
    public static LocString TransferBlockSelectionPrompt => new LocString("card_selection", "TO_TRANSFER_BLOCK");
}