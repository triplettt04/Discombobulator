using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace Discombobulator.DiscombobulatorCode.Modifications;

public class CardMethods
{
    public static bool HasZeroAttackOrBlock(CardModel card) => HasZeroAttack(card) || HasZeroBlock(card);

    public static bool HasZeroAttack(CardModel card) => card.Type == CardType.Attack && CardDamage(card) == 0;
    
    public static bool HasZeroBlock(CardModel card) => card.GainsBlock && card.DynamicVars.Block.BaseValue == 0;

    public static IEnumerable<CardModel> GetZeroAttackOrBlockCards(Player owner)
    {
        return owner.PlayerCombatState.AllCards.Where((c => c.Pile.Type != PileType.Exhaust && HasZeroAttackOrBlock(c)));
    }

    public static Decimal CardDamage(CardModel card)
    {
        DynamicVar? value;
        if (!card.DynamicVars.TryGetValue(DamageVar.defaultName, out value))
        {
            return card.DynamicVars.CalculatedDamage.BaseValue;
        }
        return value.BaseValue;
    }
    
    public static Decimal CardBlock(CardModel card)
    {
        DynamicVar? value;
        if (!card.DynamicVars.TryGetValue(BlockVar.defaultName, out value))
        {
            return card.DynamicVars.CalculatedBlock.BaseValue;
        } 
        return value.BaseValue;
    }
    
    public static List<string> VarNames(CardModel card,  bool? isDamage = null)
    {
        List<string> varNames = [];
        
        Log.Info("--- VARNAMES ---");
        foreach (var x in card.DynamicVars)
        {
            Log.Info("VarName: " + x.Key + " Value: " + x.Value);
        }

        if (isDamage != true)
        {
            if (card.DynamicVars.ContainsKey(BlockVar.defaultName))
                varNames.Add(BlockVar.defaultName);
            if (card.DynamicVars.ContainsKey(CalculatedBlockVar.defaultName))
                varNames.Add(CalculatedBlockVar.defaultName);
        }

        if (isDamage != false)
        {
            if (card.DynamicVars.ContainsKey(DamageVar.defaultName))
                varNames.Add(DamageVar.defaultName);
            if (card.DynamicVars.ContainsKey(CalculatedDamageVar.defaultName))
                varNames.Add(CalculatedDamageVar.defaultName);
        }
        
        return varNames;
    }
}