# UncappedUpgrades

A [BepInEx](https://github.com/BepInEx/BepInEx) IL2CPP mod for **The Spell Brigade** that removes the limits on stat upgrades in the level-up shop.

See [thunderstore/README.md](thunderstore/README.md) for the player-facing description and config.

## How it works

Each character stat upgrade (`CharacterImprovementItem`) has a `statLimitForOffer`. `CanStillOffer(stats)` returns true when there's no limit or the player's stat is still below it. The shop filters character upgrades in `ImprovementInventoryGenerator.GetAllPossibleCharacterImprovements`, and the compiler inlined `CanStillOffer` into its filter lambda `<GetAllPossibleCharacterImprovements>b__33_2`. So the mod hooks both.

- `LimitMultiplier` 0: the filter returns true (always offered).
- `LimitMultiplier` > 0: the limit is temporarily multiplied while the game's own check runs, then restored.
- Stats listed in `KeepLimitsFor` use the vanilla check.

The host's settings are sent to clients at run start (Netcode named message) and used for that run.

## Building

```powershell
.\package.ps1 -GameDir "D:\SteamLibrary\steamapps\common\The Spell Brigade"
```

## License

MIT, see [LICENSE](LICENSE).
