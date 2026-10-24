# UncappedUpgrades

In The Spell Brigade, each stat upgrade in the level-up shop has a limit. Once your stat reaches it, the shop stops offering that upgrade. **UncappedUpgrades removes those limits**, so you can keep stacking:

- Damage, Spell Size, Cast Speed
- Critical Chance, Critical Damage
- Max Health, Health Regeneration, Armor, Dodge
- Move Speed, Mana Pull, Mana Gain, Luck
- every other stat upgrade the shop has

You can also keep the limits but raise them, or keep the normal limit on stats you choose.

## Things to know

- **Dodge** past 100% means you never get hit, and very high cast speed or spell size can make the game busy. Use `KeepLimitsFor` if you'd rather keep some stats capped.
- Only the shop's limits change. The upgrades themselves and their values are the game's own.

## Multiplayer

Each player's own game builds their level-up shop, so **everyone should have the mod**. The **host's settings apply to everyone** for the run. Your own config file isn't changed and applies again after the run.

## Config

| Setting | Default | What it does |
|---|---|---|
| Enabled | true | Remove or raise the limits. Off = the game's normal limits. |
| LimitMultiplier | 0 | 0 = no limits. Otherwise the game's limits are multiplied by this (2 = twice as high). |
| KeepLimitsFor | (empty) | Stats that keep the normal limit, comma-separated, e.g. `Dodge, Armor`. |

Stat names for `KeepLimitsFor`: Damage, Size, FireRate (cast speed), MovementSpeed, MaxHealth, HealthRegeneration, Armor, Dodge, CriticalChance, CriticalDamage, Luck, XPGain (mana gain), PickupRadius (mana pull), Range, Speed, LifeTime, Projectiles, Rerolls, Revives, ReviveSpeed, GoldGain.

Works with MoreWizards, StarterSpells, TripleInfusion, ReviveRegen, RunSaves and BetterNetcode.
