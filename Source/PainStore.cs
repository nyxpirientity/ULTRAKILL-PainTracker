using UnityEngine;
using Nyxpiri.ULTRAKILL.NyxLib;
using System;

namespace Nyxpiri.ULTRAKILL.PainTracker
{
    public static class PainStoreHeckExtension
    {
        public static PainStore GetPainStore(this Heck heck)
        {
            return heck.GetMonoByIndex<PainStore>(PainStore.MonoRegistrarIndex);
        }
    }
    
    /* where heck itself stores its absorbed pain */
    public class PainStore : MonoBehaviour
    {
        public Heck Heck { get; private set; } = null;
        public float Pain { get; private set; } = 0.0f;
        public static int MonoRegistrarIndex { get; private set; }
        public int PainMeterRequests { get; private set; } = 0;
        public GameObject PainMeterGo { get; private set; } = null;
        public PainMeter PainMeter { get; private set; } = null;
        public int NumPainPerceptors { get; private set; } = 0;

        public void AddPain(float amount)
        {
            if (amount > 0.0f)
            {
                amount = (amount / Mathf.Max((Pain - 100.0f) / 100.0f, 1.0f));
            }
            
            Pain = Mathf.Max(0.0f, Pain + amount);
        }

        public void RegisterPainPerceptor()
        {
            NumPainPerceptors += 1;
        }

        public void UnregisterPainPerceptor()
        {
            NumPainPerceptors -= 1;
        }

        public void RequestShowPainMeter()
        {
            PainMeterRequests += 1;
        }

        public void RetractRequestShowPainMeter()
        {
            PainMeterRequests -= 1;
        }

        protected void Awake()
        {
            Heck = Heck.Itself;
        }

        protected void Start()
        {
            NewCheckpointDetector();    

            if (Assets.HeatResistancePrefabWithoutHeatResistance != null && CanvasController.Instance != null)
            {
                Assert.IsNotNull(Assets.HeatResistancePrefabWithoutHeatResistance);
                Assert.IsNotNull(Assets.HeatResistancePrefabWithoutHeatResistance.transform);
                Assert.IsNotNull(Assets.HeatResistancePrefabWithoutHeatResistance.transform.GetChild(0));
                Assert.IsNotNull(Assets.HeatResistancePrefabWithoutHeatResistance.transform.GetChild(0).gameObject);
                Assert.IsNotNull(CanvasController.Instance);
                Assert.IsNotNull(CanvasController.Instance.transform);

                PainMeterGo = GameObject.Instantiate(Assets.HeatResistancePrefabWithoutHeatResistance.transform.GetChild(0).gameObject, CanvasController.Instance.transform);
                PainMeter = PainMeterGo.AddComponent<PainMeter>();   
                PainMeterGo.SetActive(true);
            }
        }

        private void NewCheckpointDetector()
        {
            CheckpointDetector = new GameObject();
            CheckpointDetector.AddComponent<DestroyOnCheckpointRestart>();
        }

        protected void Update()
        {
            if (CheckpointDetector == null)
            {
                NewCheckpointDetector();
                
                if (Options.ResetPainOnCheckpointRestart.Value)
                {
                    Pain = 0.0f;
                }
            }

            if (PainMeterGo != null)
            {
                if (PainMeterRequests > 0 && (Pain >= 0.1f || Options.ShowPainMeterEvenIfNoPain.Value) && !Options.AlwaysHidePainMeter.Value)
                {
                    PainMeterGo.SetActive(true);
                }
                else
                {
                    PainMeterGo.SetActive(false);
                }
            }

            if (Options.AutoRequestPainMeter.Value)
            {
                if (!_RequestedShowPainMeter)
                {
                    RequestShowPainMeter();
                    _RequestedShowPainMeter = true;
                }
            }
            else
            {
                if (_RequestedShowPainMeter)
                {
                    RetractRequestShowPainMeter();
                    _RequestedShowPainMeter = false;
                }
            }
        }

        protected void FixedUpdate()
        {
            float painRot = 1.0f + (Mathf.Pow(NumPainPerceptors, 1.05f)) * 0.15f;
            painRot = Mathf.Max(0.0f, painRot);
            AddPain(-painRot * Time.fixedDeltaTime);
        }

        protected void OnDestroy()
        {
            
        }

        protected void OnEnable()
        {
            Cybergrind.PostCybergrindNextWave += OnCybergrindNextWave;
        }

        protected void OnDisable()
        {
            Cybergrind.PostCybergrindNextWave -= OnCybergrindNextWave;
        }

        private void OnCybergrindNextWave(EventMethodCancelInfo cancelInfo, EndlessGrid endlessGrid)
        {
            if (Options.ResetPainOnCybergrindWaveChange.Value)
            {
                Pain = 0.0f;
            }
        }

        internal static void Initialize()
        {
            MonoRegistrarIndex = Heck.MonoRegistrar.Register<PainStore>();
        }

        private GameObject CheckpointDetector = null;
        private bool _RequestedShowPainMeter = false;
    }
}