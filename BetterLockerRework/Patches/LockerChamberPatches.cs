using BetterLockers;
using HarmonyLib;
using LabApi.Features.Console;
using MapGeneration.Distributors;
using System.Linq;

[HarmonyPatch(typeof(Locker), nameof(Locker.FillChamber))]
public static class LockerFillChamberPatch
{
	public static bool Prefix(Locker __instance, LockerChamber ch)
	{
		var cfg = Main.Instance.Config;

		if (!cfg.IsEnabled)
		{
			return true;
		}

		bool blockVanilla = cfg.DisableBaseGameItems.TryGetValue(__instance.StructureType, out bool destroy) && destroy;

		if (!blockVanilla)
		{
			return true;
		}

		if (cfg.LockerSpawns.TryGetValue(__instance.StructureType, out var spawnerList) && spawnerList.Count > 0)
		{
			var shuffledList = spawnerList.OrderBy(_ => UnityEngine.Random.value).ToList();
			bool itemSpawned = false;

			foreach (var spawner in shuffledList)
			{
				int existingCount = __instance.Chambers.Sum(c => c.Content.Count(item => item != null && item.ItemId.TypeId == spawner.item));

				if (spawner.maxamountinlocker > 0 && existingCount >= spawner.maxamountinlocker)
				{
					if (cfg.Debug)
						Logger.Info($"[LockerFillPatch] Skipped {spawner.item} in {__instance.StructureType}: MaxAmountInLocker {spawner.maxamountinlocker} reached.");
					continue;
				}

				if (UnityEngine.Random.Range(1, 101) <= spawner.chance)
				{
					ch.SpawnItem(spawner.item, spawner.amount);
					itemSpawned = true;

					if (cfg.Debug)
						Logger.Info($"[LockerFillPatch] Spawned {spawner.item} x{spawner.amount} in {__instance.StructureType}.");
					break;
				}
			}

			if (!itemSpawned)
			{
				var fallbackSpawner = shuffledList.First();
				ch.SpawnItem(fallbackSpawner.item, fallbackSpawner.amount);

				if (cfg.Debug)
					Logger.Info($"[LockerFillPatch] Forced spawn: {fallbackSpawner.item} x{fallbackSpawner.amount} in {__instance.StructureType}.");
			}

			return false;
		}

		return true;
	}
}
