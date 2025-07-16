# BetterLockerRework
 Allows to spawn your own items inside lockers.
## Config
```
is_enabled: true
# Keep this off unless you want your console spammed.
debug: false
# Stop basegame items from spawning in lockers. StructureTypes that are not in this list will default to false (StandardLocker, LargeGunLocker, ScpPedestal, SmallWallCabinet, ExperimentalWeaponLocker)
disable_base_game_items:
  StandardLocker: true
# Available types: StandardLocker, LargeGunLocker, ScpPedestal, SmallWallCabinet, ExperimentalWeaponLocker
locker_spawns:
  StandardLocker:
  - item: GunCOM15
    chance: 100
    amount: 1
    maxamountinlocker: 1
  - item: KeycardGuard
    chance: 100
    amount: 1
    maxamountinlocker: 1
  LargeGunLocker:
  - item: MicroHID
    chance: 1
    amount: 1
    maxamountinlocker: 1
  ExperimentalWeaponLocker:
  - item: MicroHID
    chance: 100
    amount: 1
    maxamountinlocker: 1
```

Original Plugin: https://github.com/BruteForceMaestro/BetterLockers
