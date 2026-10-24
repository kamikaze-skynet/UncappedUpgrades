using System;
using System.Collections.Generic;
using HarmonyLib;

namespace UncappedUpgrades;

/// <summary>
/// CharacterImprovementItem.CanStillOffer(stats): vanilla = "no limit set, or your stat is still below the limit".
/// Here: no limit (LimitMultiplier 0) or a multiplied limit, except for stats listed in KeepLimitsFor.
/// </summary>
[HarmonyPatch]
internal static class LimitPatches
{
    private static string keepCacheKey;
    private static HashSet<StatType> keep = new();
    private static bool loggedOnce;

    private static bool Kept(StatType stat)
    {
        string raw = HostSettings.KeepLimitsFor ?? "";
        if (raw != keepCacheKey)
        {
            keepCacheKey = raw;
            keep = new HashSet<StatType>();
            foreach (var part in raw.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                if (Enum.TryParse(part.Trim(), true, out StatType t)) keep.Add(t);
                else Plugin.Log.LogWarning($"KeepLimitsFor: unknown stat \"{part}\".");
        }
        return keep.Contains(stat);
    }

    // The shop's filter (ImprovementInventoryGenerator.GetAllPossibleCharacterImprovements) has CanStillOffer compiled
    // into a lambda, so hooking CanStillOffer alone wouldn't change the shop. Both are hooked with the same logic.

    [HarmonyPatch(typeof(ImprovementInventoryGenerator), nameof(ImprovementInventoryGenerator._GetAllPossibleCharacterImprovements_b__33_2))]
    [HarmonyPrefix]
    private static bool ShopFilter_Prefix(CharacterImprovementItem improvement, ref bool __result, out float __state) =>
        Before(improvement, ref __result, out __state);

    [HarmonyPatch(typeof(ImprovementInventoryGenerator), nameof(ImprovementInventoryGenerator._GetAllPossibleCharacterImprovements_b__33_2))]
    [HarmonyPostfix]
    private static void ShopFilter_Postfix(CharacterImprovementItem improvement, float __state) => After(improvement, __state);

    [HarmonyPatch(typeof(CharacterImprovementItem), nameof(CharacterImprovementItem.CanStillOffer))]
    [HarmonyPrefix]
    private static bool CanStillOffer_Prefix(CharacterImprovementItem __instance, ref bool __result, out float __state) =>
        Before(__instance, ref __result, out __state);

    [HarmonyPatch(typeof(CharacterImprovementItem), nameof(CharacterImprovementItem.CanStillOffer))]
    [HarmonyPostfix]
    private static void CanStillOffer_Postfix(CharacterImprovementItem __instance, float __state) => After(__instance, __state);

    /// <summary>False with __result = true: offer it. True: let the game check (with a raised limit if multiplied).</summary>
    private static bool Before(CharacterImprovementItem item, ref bool __result, out float __state)
    {
        __state = float.NaN;
        try
        {
            if (item == null || !HostSettings.Enabled) return true;
            float limit = item.statLimitForOffer;
            if (limit == 0f) return true;               // no limit on this upgrade anyway
            if (Kept(item.Stat)) return true;           // keeps the game's limit

            float mult = HostSettings.LimitMultiplier;
            if (mult <= 0f)
            {
                if (!loggedOnce) { loggedOnce = true; Plugin.Log.LogInfo("Upgrade limits removed: stat upgrades keep being offered."); }
                __result = true;
                return false;
            }
            __state = limit;                            // run the game's check with the multiplied limit
            item.statLimitForOffer = limit * mult;
            return true;
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Upgrade limit: {ex.Message}");
            return true;
        }
    }

    private static void After(CharacterImprovementItem item, float __state)
    {
        if (float.IsNaN(__state) || item == null) return;
        try { item.statLimitForOffer = __state; } catch { }
    }
}
