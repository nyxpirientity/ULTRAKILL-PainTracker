using UnityEngine;
using BepInEx;
using Nyxpiri.ULTRAKILL.NyxLib;

namespace Nyxpiri.ULTRAKILL.PainTracker
{
    [BepInPlugin("com.nyxpiri.bepinex.plugins.ultrakill.pain-tracker", "Pain Tracker", "0.0.0.1")]
    [BepInProcess("ULTRAKILL.exe")]
    public class PainTracker: BaseUnityPlugin
    {
        protected void Awake()
        {
            Log.Initialize(Logger);
        }

        protected void Start()
        {
            PainStore.Initialize();
        }

        protected void Update()
        {

        }

        protected void LateUpdate()
        {

        }
    }
}
