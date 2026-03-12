using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Nyxpiri.ULTRAKILL.NyxLib;

namespace Nyxpiri.ULTRAKILL.PainTracker
{
    public class PainMeter : MonoBehaviour
    {
        public Heck Heck { get; private set; } = null;
        public PainStore PainStore { get; private set; } = null;

        [SerializeField] private bool Configured = false;

        public bool Enabled { get => Heck.Itself.GetPainStore().PainMeterRequests >= 1; }
        public bool Disabled { get => !Enabled; }
        [SerializeField] public Slider Meter { get; private set; }
        public GameObject Fill { get; private set; }
        [SerializeField] public TextMeshProUGUI MeterPercentage { get; private set; }
        public TextMeshProUGUI MeterLabel { get; private set; }
        public RectTransform RectTransform { get; private set; }

        protected void Awake()
        {
            Heck = Heck.Itself;
            PainStore = Heck.Itself.GetPainStore();

            if (!Configured)
            {
                Configure();
            }
            
            Configured = true;
        }

        private void Configure()
        {
            GameObject.Destroy(transform.Find("Flavor Text").gameObject);
            GameObject.Destroy(transform.Find("Warning").gameObject);
            var maybeHeatRes = GetComponent<HeatResistance>();
            
            if (maybeHeatRes != null)
            {
                GameObject.Destroy(maybeHeatRes);
            }

            var audioSources = GetComponents<AudioSource>();

            foreach (var audio in audioSources)
            {
                audio.Stop();
                audio.enabled = false;
            }

            Meter = transform.Find("Meter").gameObject.GetComponent<Slider>();
            Fill = transform.Find("Meter/Fill Area/Fill").gameObject;
            MeterPercentage = transform.Find("Meter/Fill Area/Fill/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
            MeterLabel = transform.Find("Meter/Label").gameObject.GetComponent<TextMeshProUGUI>();
            Meter.minValue = 0.0f;
            Meter.maxValue = 100.0f;
            Meter.direction = Slider.Direction.RightToLeft;

            RectTransform = GetComponent<RectTransform>();
            RectTransform.anchorMin = new Vector2(1.0f, 0.0f);
            RectTransform.anchorMax = new Vector2(1.0f, 0.0f);
            RectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
            RectTransform.anchoredPosition = new Vector2(-25.0f, 225.0f);
            RectTransform.sizeDelta = RectTransform.sizeDelta - new Vector2(100.0f, 40.0f);
        }

        protected void Start()
        {
            
        }

        protected void Update()
        {
            Meter.value = PainStore.Pain;

            float painAlpha = NyxMath.NormalizeToRange(PainStore.Pain, Meter.minValue, Meter.maxValue);
            Fill.GetComponent<CanvasRenderer>().SetColor(Color.Lerp(Color.white, Color.red, painAlpha));
            MeterPercentage.text = $"{painAlpha * 100.0f:F2}%";
            MeterLabel.text = $"{(painAlpha < 0.5f ? "PAIN" : "AGONY")} - {painAlpha * 100.0f:F1}%";
        }

        protected void FixedUpdate()
        {

        }
        
        protected void OnDestroy()
        {
            
        }

        protected void OnEnable()
        {
            
        }
        
        protected void OnDisable()
        {
            
        }
    }
}