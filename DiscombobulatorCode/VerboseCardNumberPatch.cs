using Discombobulator.DiscombobulatorCode.Modifications;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace Discombobulator.DiscombobulatorCode;

[HarmonyPatch(typeof(DynamicVar), nameof(DynamicVar.ToHighlightedString), typeof(bool))]
public static class VerboseCardNumberPatch
{
    [HarmonyPostfix]
    public static void Postfix(DynamicVar __instance, ref string __result)
    {
        List<string> supportedTypes = [
            DamageVar.defaultName, 
            CalculatedDamageVar.defaultName, 
            BlockVar.defaultName,
            CalculatedBlockVar.defaultName
        ];
        
        if (!supportedTypes.Contains(__instance.Name))
        {
            return;
        }
            
        string color = "gray";
        int extraDamage = (int) __instance.PreviewValue - __instance.IntValue;
        
        // Need to add something for previewing transfers
        if (extraDamage != 0)
        {
            __result = __result
                       + " [color=" + color + "](" + __instance.IntValue
                       + (extraDamage > 0 ? " + " : " - ")
                       + Math.Abs(extraDamage) + ")[/color]";
        }
    }
}
