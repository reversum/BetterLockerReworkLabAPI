
using HarmonyLib;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System;
using static PlayerList;
using static UnityEngine.UI.Selectable;

namespace BetterLockers
{
	public class Main : Plugin
	{
		private Harmony _harmony;
		public static Main Instance { get; private set; }
		public override string Name { get; } = "BetterLockerRework";
		public override string Description { get; } = "Allows to spawn your own items inside lockers.";
		public override string Author => "YannikAufDie1";
		public override Version Version { get; } = new Version(1, 0, 0, 0);
		public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
		public Config Config { get; private set; }
		public override void Enable()
		{
			LoadConfigs();
			if (!Config.IsEnabled)
			{
				Logger.Info("Plugin is set to not start...");
				return;
			}
			Instance = this;
			_harmony = new Harmony("lockerreworklabapi.yannik");
			_harmony.PatchAll();
		}

		public override void Disable()
		{
			Instance = this;
			_harmony.UnpatchAll("lockerreworklabapi.yannik");
		}

		public override void LoadConfigs()
		{
			this.TryLoadConfig("config.yml", out Config config);
			Config = config ?? new Config();
		}
	}
}
