
using HarmonyLib;
using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System;
using static UnityEngine.UI.Selectable;

namespace BetterLockers
{
	public class Main : Plugin
	{
		private Harmony _harmony;
		private static readonly Main Singleton = new();
		public override string Name { get; } = "BetterLockerRework";
		public override string Description { get; } = "Allows to spawn your own items inside lockers.";
		public override string Author => "YannikAufDie1";
		public override Version Version { get; } = new Version(1, 0, 0, 0);
		public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
		public static Main Instance => Singleton;
		public Config Config { get; private set; }
		public override void Enable()
		{
			_harmony = new Harmony("lockerrework.yannik");
			_harmony.PatchAll();
		}

		public override void Disable()
		{
			_harmony.UnpatchAll("lockerrework.yannik");
		}

		public override void LoadConfigs()
		{
			this.TryLoadConfig("config.yml", out Config config);
			Config = config ?? new Config();
		}
	}
}
