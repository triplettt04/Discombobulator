using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Discombobulator.DiscombobulatorCode.Cards;

public class TransferCmd
{
    public static async Task Transfer(
        CardModel giver,
        IEnumerable<CardModel> targets,
        decimal transferAmount,
        string varName
    )
    {
        decimal amount = Math.Min(giver.DynamicVars[varName].BaseValue, transferAmount);
        
        giver.DynamicVars[varName].BaseValue -= amount;
        IEnumerable<decimal> previews = new[] {giver.DynamicVars[varName].BaseValue};
        
        foreach (CardModel target in targets)
        {
            target.DynamicVars[varName].BaseValue += amount;
        }
        
        NCard.FindOnTable(giver)?.ReloadOverlay();
        foreach (CardModel target in targets)
        {
            NCard.FindOnTable(target)?.ReloadOverlay();
        }
    }

    public static bool CanTransferDamage(CardModel cardModel)
    {
        //Determine if the card is an attack
        return cardModel.Type == CardType.Attack; // && cardModel.DynamicVars.Damage.BaseValue > 0
    }
    
    public static bool CanTransferBlock(CardModel cardModel)
    {
        //Determine if the card gains block
        return cardModel.GainsBlock;
    }
}