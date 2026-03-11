using UnityEngine;
using Nyxpiri.ULTRAKILL.NyxLib;

namespace Nyxpiri.ULTRAKILL.PainTracker
{
    public class PlayerPain : MonoBehaviour
    {
        public static int MonoRegistrarIdx { get; private set; }

        protected void Start()
        {
            Player = NewMovement.Instance;
        }

        protected void FixedUpdate()
        {
            var painValue = Mathf.Lerp(0.0f, -1.0f, NyxMath.NormalizeToRange(Player.hp, -100.0f, 100.0f));
            Heck.Itself.GetPainStore().AddPain((float)painValue * Time.fixedDeltaTime);
        }

        protected void OnEnable()
        {
            PlayerEvents.PostHurt += PostPlayerHurt;
        }

        protected void OnDisable()
        {
            PlayerEvents.PostHurt -= PostPlayerHurt;
        }

        NewMovement Player = null;

        private void PostPlayerHurt(EventMethodCancelInfo cancelInfo, PlayerComponents player, int unprocessedDamage, int processedDamage, bool invincible, float scoreLossMultiplier, bool explosion, bool instablack, float hardDamageMultiplier, bool ignoreInvincibility)
        {
            Heck.Itself.GetPainStore().AddPain((float)processedDamage * 0.01f);
        }

        internal static void Initialize()
        {
            MonoRegistrarIdx = PlayerComponents.MonoRegistrar.Register<PlayerPain>();
        }
    }
}