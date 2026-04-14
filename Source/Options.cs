using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using Nyxpiri.ULTRAKILL.NyxLib;

namespace Nyxpiri.ULTRAKILL.PainTracker
{
    public static class Options
    {
        public static ConfigEntry<bool> AutoRequestPainMeter = null;
        public static ConfigEntry<bool> ShowPainMeterEvenIfNoPain = null;
        public static ConfigEntry<bool> AlwaysHidePainMeter = null;

        public static ConfigEntry<bool> ResetPainOnCybergrindWaveChange = null;
        public static ConfigEntry<bool> ResetPainOnCheckpointRestart = null;

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            _config = plugin.Config;

            _configFileManager = plugin.gameObject.AddComponent<ConfigFileManager>();
            _configFileManager.Initialize(_config);
            _configFileManager.OnReload += Reload;

            AutoRequestPainMeter = _config.Bind("Meter", "ShowPainMeterDespiteNoRequests", false, "Shows pain meter even when no mods request it to be shown");
            ShowPainMeterEvenIfNoPain = _config.Bind("Meter", "ShowPainMeterEvenIfNoPain", false, "Shows pain meter even when there is no pain");
            AlwaysHidePainMeter = _config.Bind("Meter", "AlwaysHidePainMeter", false, "Hides pain meter even when mods request it to be shown");
            ResetPainOnCybergrindWaveChange = _config.Bind("Resetting", "ResetPainOnCybergrindWaveChange", true, "");
            ResetPainOnCheckpointRestart = _config.Bind("Resetting", "ResetPainOnCheckpointRestart", true, "");
        }

        private static void Reload()
        {
        }

        private static ConfigFile _config = null;
        private static ConfigFileManager _configFileManager = null;
    }
}
