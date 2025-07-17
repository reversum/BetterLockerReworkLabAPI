using BetterLockers;
using GameCore;
using HarmonyLib;
using LabApi.Features.Console;
using MapGeneration.Distributors;
using System;
using System.Linq;
using System.Reflection;

[HarmonyPatch(typeof(Locker), nameof(Locker.FillChamber))]
public static class LockerFillChamberPatch
{
	public static bool Prefix(Locker __instance, LockerChamber ch)
	{
		var cfg = Main.Instance.Config;

		bool blockVanilla = cfg.DisableBaseGameItems.TryGetValue(__instance.StructureType, out bool destroy) && destroy;

		if (!blockVanilla)
			return true;

		if (cfg.LockerSpawns.TryGetValue(__instance.StructureType, out var spawnerList) && spawnerList.Count > 0)
		{
			var shuffledList = spawnerList.OrderBy(_ => UnityEngine.Random.value).ToList();

			var uciAssembly = AppDomain.CurrentDomain.GetAssemblies()
				.FirstOrDefault(a => a.GetName().Name.Contains("UncomplicatedCustomItems"));

			Type uciUtilsType = null;
			Type summonedCustomItemType = null;

			if (uciAssembly != null)
			{
				uciUtilsType = uciAssembly.GetType("UncomplicatedCustomItems.API.Utilities");
				summonedCustomItemType = uciAssembly.GetType("UncomplicatedCustomItems.API.Features.SummonedCustomItem");
			}

			foreach (var spawner in shuffledList)
			{
				int existingCount = 0;

				foreach (var chamber in __instance.Chambers)
				{
					existingCount += chamber.Content.Count(item => item != null && item.ItemId.TypeId.ToString() == spawner.item);
				}

				if (spawner.maxamountinlocker > 0 && existingCount >= spawner.maxamountinlocker)
				{
					if (cfg.Debug)
						Logger.Info($"Skipped {spawner.item} in {__instance.StructureType}: MaxAmountInLocker {spawner.maxamountinlocker} reached.");
					continue;
				}

				if (UnityEngine.Random.Range(1, 101) <= spawner.chance)
				{
					if (!spawner.uciitem)
					{
						if (Enum.TryParse(spawner.item, ignoreCase: true, out ItemType itemType))
						{
							ch.SpawnItem(itemType, spawner.amount);
							break;
						}
						else
						{
							Logger.Warn($"Invalid ItemType '{spawner.item}' for locker {__instance.StructureType}");
						}
					}
					else if (uciUtilsType != null && summonedCustomItemType != null)
					{
						var tryGetMethod = uciUtilsType.GetMethod("TryGetCustomItemByName", BindingFlags.Public | BindingFlags.Static);
						var parameters = new object[] { spawner.item, null };

						bool found = (bool)tryGetMethod.Invoke(null, parameters);
						var customItem = parameters[1];

						if (!found || customItem == null)
						{
							Logger.Warn($"CustomItem '{spawner.item}' not found.");
							continue;
						}

						for (int i = 0; i < spawner.amount; i++)
						{
							var pos = ch.transform.position + ch.transform.up * 0.1f;
							var rot = ch.transform.rotation;

							Activator.CreateInstance(summonedCustomItemType, customItem, pos, rot);
						}
						break;
					}
					else
					{
						Logger.Warn($"UCI not present: Skipping custom item '{spawner.item}'");
					}
				}
			}

			return false;
		}
		return false;
	}
}

