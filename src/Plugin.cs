using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace UncappedUpgrades;

/// <summary>
/// Removes the limits on stat upgrades in the level-up shop.
///
/// Each stat upgrade (damage, spell size, cast speed, luck, crit, max health, ...) has a "stat limit for offer":
/// once your stat reaches it, the shop stops offering that upgrade (CharacterImprovementItem.CanStillOffer).
/// This mod removes that limit, or multiplies it. The stats themselves aren't capped by the game.
///
/// Each player's own game builds their shop, so everyone should have the mod; the host's settings apply to all.
/// </summary>
[BepInPlugin(Guid, Name, Version)]
public class Plugin : BasePlugin
{
    public const string Guid = "girthy_mods.UncappedUpgrades";
    public const string Name = "UncappedUpgrades";
    public const string Version = "1.0.0";

    internal static new ManualLogSource Log;

    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<float> LimitMultiplier;
    internal static ConfigEntry<string> KeepLimitsFor;

    public override void Load()
    {
        Log = base.Log;

        Enabled = Config.Bind("Limits", "Enabled", true,
            "Remove (or raise) the limits on stat upgrades in the level-up shop.");
        LimitMultiplier = Config.Bind("Limits", "LimitMultiplier", 0f,
            new ConfigDescription("0 = no limits at all. Otherwise the game's limits are multiplied by this (2 = twice as high).",
                new AcceptableValueRange<float>(0f, 100f)));
        KeepLimitsFor = Config.Bind("Limits", "KeepLimitsFor", "",
            "Stats that keep the game's normal limit, comma-separated (e.g. \"Dodge, Armor\"). Names: Damage, Size, FireRate, " +
            "MovementSpeed, MaxHealth, HealthRegeneration, Armor, Dodge, CriticalChance, CriticalDamage, Luck, XPGain, PickupRadius, " +
            "Range, Speed, LifeTime, Projectiles, Rerolls, Revives, ReviveSpeed, GoldGain.");

        var harmony = new Harmony(Guid);
        harmony.PatchAll(typeof(LimitPatches));
        harmony.PatchAll(typeof(HostSettings));
        Log.LogInfo($"{Name} {Version} ready.");
    }
}
