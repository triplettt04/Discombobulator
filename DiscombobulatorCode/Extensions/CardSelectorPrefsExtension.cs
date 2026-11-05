using Discombobulator.DiscombobulatorCode.Modifications;
using Discombobulator.DiscombobulatorCode.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Discombobulator.DiscombobulatorCode.Extensions;

public static class CardSelectorPrefsExtension
{
    public static LocString TransferDamageSelectionPrompt => new LocString("card_selection", "TO_TRANSFER_DAMAGE");
    
    public static LocString TransferBlockSelectionPrompt => new LocString("card_selection", "TO_TRANSFER_BLOCK");

    public static LocString TransferSelectionPrompt(AbstractModel source, bool isDamage)
    {
        LocString prompt = isDamage
            ? TransferDamageSelectionPrompt
            : TransferBlockSelectionPrompt;
        
        if (source is CardModel)
        {
            prompt.Add(((CardModel)source).DynamicVars[TransferVar.Key]);
        }
        else if (source is DiscombobulatorPower)
        {
            prompt.Add(new TransferVar(((DiscombobulatorPower)source).Amount));
        }

        return prompt;
    }
}