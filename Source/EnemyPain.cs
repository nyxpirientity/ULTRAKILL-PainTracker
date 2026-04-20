using System;
using UnityEngine;
using Nyxpiri.ULTRAKILL.NyxLib;

namespace Nyxpiri.ULTRAKILL.PainTracker
{
    public class EnemyPain : EnemyModifier
    {
        /* 
        * by nature, the initial defaults I'm writing for this class as I write this comment are a little lore heavy and I don't really give a darn about ULTRAKILL lore at all so uhhhhhhhh
        * either way it'll probably eventually be modified to suit balance instead unless it ends up actually being balanced via my best understanding of the lore being applied
        */
        public EnemyComponents Enemy { get; private set; } = null;
        public static int MonoRegistrarIdx { get; private set; }

        public float ActivePhysicalPain = 0.0f;
        public float PhysicalSensitivity = 1.0f;

        public float ActiveMentalPain = 0.0f;
        public float ActiveHardMentalPain = 0.0f;
        public float MentalSensitivity = 1.0f;

        protected void Awake()
        {
            Enemy = GetComponent<EnemyComponents>();
            _painPerceptorRegistrationTracker = new RegistrationTracker(
                () =>
                {
                    if (Enemy.Eid.puppet || Enemy.Eid.Dead)
                    {
                        return false;  
                    }

                    Heck.Itself.GetPainStore().RegisterPainPerceptor();
                    return true;
                },
                () =>
                {
                    Heck.Itself.GetPainStore().UnregisterPainPerceptor();
                    return true;
                }
            );
        }

        protected void Start()
        {
            SpeciesType = Enemy.Eid.GetSpeciesType();
            SpeciesRank = Enemy.Eid.GetSpeciesRank();
            
            Enemy.PreHurt += OnPreHurt;
            Enemy.PostDeath += PostDeath;
            Enemy.PreEnrage += PreEnrage;

            Enemy.PreDeath += OnPreDeath;
        }

        private void PreEnrage(EventMethodCanceler canceler)
        {
            ActiveMentalPain += 0.75f;
        }

        private void OnPreDeath(EventMethodCanceler canceler, bool instakill)
        {
            if (Enemy.Eid.enemyType == EnemyType.Mindflayer)
            {
                Heck.Itself.GetPainStore().AddPain(5.0f);
            }
            else if (Enemy.Eid.enemyType == EnemyType.HideousMass)
            {
                Heck.Itself.GetPainStore().AddPain(3.5f);
            }

            _painPerceptorRegistrationTracker.Unregister();
        }

        protected void OnEnable()
        {
            TryListenForDeath();
            _painPerceptorRegistrationTracker.Register();
        }

        protected void OnDisable()
        {
            TryStopListeningForDeath();
            _painPerceptorRegistrationTracker.Unregister();
        }

        private bool ListeningForDeath = false;
        private EnemySpeciesType SpeciesType;
        private EnemySpeciesRank SpeciesRank;

        private void PostDeath(EventMethodCancelInfo cancelInfo, bool instakilled)
        {
            TryStopListeningForDeath();
        }

        private void TryListenForDeath()
        {
            if (ListeningForDeath)
            {
                return;
            }
            
            if ((Enemy.NullInvalid()?.Eid.NullInvalid()?.Dead).GetValueOrDefault(true))
            {
                return;
            }

            EnemyEvents.Death += OnAnyEnemyDeath;
            PlayerEvents.PostHurt += PostPlayerHurt;
            ListeningForDeath = true;
        }

        private void TryStopListeningForDeath()
        {
            if (!ListeningForDeath)
            {
                return;
            }

            EnemyEvents.Death -= OnAnyEnemyDeath;
            PlayerEvents.PostHurt -= PostPlayerHurt;
            ListeningForDeath = false;
        }

        private void PostPlayerHurt(EventMethodCancelInfo cancelInfo, PlayerComponents player, int unprocessedDamage, int processedDamage, bool invincible, float scoreLossMultiplier, bool explosion, bool instablack, float hardDamageMultiplier, bool ignoreInvincibility)
        {
            ActiveMentalPain = Math.Max(ActiveHardMentalPain, ActiveMentalPain - (float)processedDamage * 0.01f);
            Heck.Itself.GetPainStore().AddPain(-processedDamage * 0.005f);
        }

        private void OnAnyEnemyDeath(EnemyComponents otherEnemy)
        {
            if (SpeciesType is EnemySpeciesType.OrganicMachine)
            {
                return;
            }

            if (otherEnemy.Eid.puppet)
            {
                return;
            }

            var otherSpeciesType = otherEnemy.Eid.GetSpeciesType();
            var otherSpeciesRank = otherEnemy.Eid.GetSpeciesRank();

            float compassion = 0.0f;
            float concern = 0.0f;

            if (Enemy.Eid.enemyType is EnemyType.Filth || Enemy.Eid.enemyType is EnemyType.Stray || Enemy.Eid.enemyType is EnemyType.Schism || Enemy.Eid.enemyType is EnemyType.Soldier)
            {
                if (otherEnemy.Eid.enemyType is EnemyType.Filth || otherEnemy.Eid.enemyType is EnemyType.Stray || otherEnemy.Eid.enemyType is EnemyType.Schism || otherEnemy.Eid.enemyType is EnemyType.Soldier)
                {
                    compassion += 0.25f; // they don't attack each other even with enemies attack enemies on soooooo
                }   
            }
            
            if (Enemy.Eid.enemyType == otherEnemy.Eid.enemyType)
            {
                compassion += 0.4f;
            }

            float lesserConcernScale = 0.0f;
            float greaterConcernScale = 0.0f;
            float supremeConcernScale = 0.0f;
            float primeConcernScale = 0.0f;

            switch (SpeciesRank)
            {
                case EnemySpeciesRank.NotApplicable:
                    lesserConcernScale = 0.0f;
                    greaterConcernScale = 0.0f;
                    supremeConcernScale = 0.0f;
                    primeConcernScale = 0.0f;
                    break;
                case EnemySpeciesRank.Lesser:
                    lesserConcernScale = 0.25f;
                    greaterConcernScale = 0.35f;
                    supremeConcernScale = 0.5f;
                    primeConcernScale = 0.65f;
                    break;
                case EnemySpeciesRank.Greater:
                    lesserConcernScale = 0.05f;
                    greaterConcernScale = 0.5f;
                    supremeConcernScale = 0.75f;
                    primeConcernScale = 1.0f;
                    break;
                case EnemySpeciesRank.Supreme:
                    lesserConcernScale = 0.01f;
                    greaterConcernScale = 0.2f;
                    supremeConcernScale = 0.5f;
                    primeConcernScale = 0.75f;
                    break;
                case EnemySpeciesRank.Prime:
                    lesserConcernScale = 0.0f;
                    greaterConcernScale = 0.025f;
                    supremeConcernScale = 0.05f;
                    primeConcernScale = 0.25f;
                    break;
            }

            if (SpeciesType is EnemySpeciesType.Machine && otherSpeciesType is EnemySpeciesType.Machine)
            {
                switch (otherSpeciesRank)
                {
                    case EnemySpeciesRank.NotApplicable:
                        break;
                    case EnemySpeciesRank.Lesser:
                        concern += lesserConcernScale * 0.25f;
                        break;
                    case EnemySpeciesRank.Greater:
                        concern += greaterConcernScale * 0.5f;
                        break;
                    case EnemySpeciesRank.Supreme:
                        concern += supremeConcernScale;
                        break;
                    case EnemySpeciesRank.Prime:
                        concern += float.PositiveInfinity;
                        break;
                }
            }
            else
            {
                switch (otherSpeciesRank)
                {
                    case EnemySpeciesRank.NotApplicable:
                        break;
                    case EnemySpeciesRank.Lesser:
                        concern += lesserConcernScale;
                        break;
                    case EnemySpeciesRank.Greater:
                        concern += greaterConcernScale;
                        break;
                    case EnemySpeciesRank.Supreme:
                        concern += supremeConcernScale * (((SpeciesType == otherSpeciesType && SpeciesType == EnemySpeciesType.Angel) && (SpeciesRank == EnemySpeciesRank.Greater || SpeciesRank == EnemySpeciesRank.Lesser)) ? 10.0f : 1.0f);
                        break;
                    case EnemySpeciesRank.Prime:
                        concern += primeConcernScale;
                        break;
                }
            }

            var mentalPain = (concern + compassion) * MentalSensitivity * Options.MentalPainMultiplier.Value;
            MentalSensitivity = Mathf.Max(0.2f, MentalSensitivity - mentalPain);
            ActiveMentalPain += mentalPain;
            ActiveHardMentalPain += mentalPain * 0.2f;
        }

        protected void FixedUpdate()
        {
            ActivePhysicalPain = Mathf.MoveTowards(ActivePhysicalPain, -0.05f, Time.fixedDeltaTime * 0.5f);
            PhysicalSensitivity = Mathf.MoveTowards(PhysicalSensitivity, 1.0f, Time.fixedDeltaTime * 1.25f);

            ActiveMentalPain = Mathf.MoveTowards(ActiveMentalPain, ActiveHardMentalPain, Time.fixedDeltaTime * 0.4f);
            ActiveHardMentalPain = Mathf.MoveTowards(ActiveHardMentalPain, -0.05f, Time.fixedDeltaTime * 0.25f);
            MentalSensitivity = Mathf.MoveTowards(MentalSensitivity, 1.0f, Time.fixedDeltaTime * 1.25f);

            if (Enemy.Eid.Dead)
            {
                ActivePhysicalPain = 0.0f;
                ActiveMentalPain = 0.0f;
                return;
            }
            
            ActivePhysicalPain = Mathf.Min(ActivePhysicalPain, Options.PhysicalPainPerEnemyCap.Value);
            ActiveMentalPain = Mathf.Min(ActiveMentalPain, Options.MentalPainPerEnemyCap.Value);
            ActiveHardMentalPain = Mathf.Min(ActiveHardMentalPain, Options.MentalPainPerEnemyCap.Value);

            float painScalar = 1.0f;

            if (Enemy.Eid.puppet)
            {
                painScalar = 0.0f;
            }

            switch (Enemy.Eid.enemyType)
            {
                // they've probably ascended beyond the concepts of pain, mild annoyance for them at the worst.
                case EnemyType.VeryCancerousRodent:
                case EnemyType.CancerousRodent:
                case EnemyType.Mandalore:
                    ActivePhysicalPain = -1.0f;
                    ActiveMentalPain = -1.0f;
                break;
                case EnemyType.SisyphusPrime:
                    ActiveMentalPain = 0.0f;
                    painScalar = 0.25f;
                    break;
                default:
                    break;
            }

            Heck.Itself.GetPainStore().AddPain((ActivePhysicalPain + ActiveMentalPain) * painScalar * Time.fixedDeltaTime);
        }

        private void OnPreHurt(EventMethodCanceler canceler, GameObject target, Vector3 force, Vector3? hitPoint, float multiplier, bool tryForExplode, float critMultiplier, GameObject sourceWeapon, bool ignoreTotalDamageTakenMultiplier, bool fromExplosion)
        {
            if (!ListeningForDeath)
            {
                return;
            }

            float pain = ((Mathf.Pow(multiplier + (multiplier * critMultiplier), 0.5f) * PhysicalSensitivity) / (Mathf.Pow(Enemy.InitialHealth, 0.5f))) * 2.0f;

            PhysicalSensitivity = Mathf.Max(0.2f, PhysicalSensitivity - (pain * 0.5f));

            ActivePhysicalPain += pain * Options.PhysicalPainMultiplier.Value;
        }

        internal static void Initialize()
        {
            MonoRegistrarIdx = EnemyComponents.MonoRegistrar.Register<EnemyPain>();
        }

        private RegistrationTracker _painPerceptorRegistrationTracker;
    }
}