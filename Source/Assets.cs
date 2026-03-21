using System.IO;
using HarmonyLib;
using TMPro;
using Nyxpiri.ULTRAKILL.NyxLib.Diagnostics.Debug;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using Nyxpiri.ULTRAKILL.NyxLib;

namespace Nyxpiri.ULTRAKILL.PainTracker
{
    internal static class Assets
    {
        public static GameObject HeatResistancePrefabWithoutHeatResistance { get; private set; } = null;
        
        public static void Initialize()
        {
            ScenesEvents.OnSceneWasLoaded += OnSceneWasLoaded;
        }

        private static void OnSceneWasLoaded(Scene scene, string sceneName)
        {
            if (HeatResistancePrefabWithoutHeatResistance == null)
            {
                var possibleHeatResistance = UnityEngine.Object.FindAnyObjectByType<HeatResistance>(FindObjectsInactive.Include);
                
                if (possibleHeatResistance != null)
                {
                    HeatResistancePrefabWithoutHeatResistance = UnityEngine.Object.Instantiate(possibleHeatResistance.gameObject.transform.parent.gameObject, null, false);
                    HeatResistancePrefabWithoutHeatResistance.SetActive(false);
                    UnityEngine.Object.DontDestroyOnLoad(HeatResistancePrefabWithoutHeatResistance);
                    GameObject.Destroy(HeatResistancePrefabWithoutHeatResistance.GetComponentInChildren<HeatResistance>());
                }
            }
        }
    }
}