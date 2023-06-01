using System;
using Assets.Scripts.Model.Levels;
using JetBrains.Annotations;
using Model.HealthSystem;
using Model.MovingObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Enemies
{
    public class Chaser : Enemy
    {
        [SerializeField] private float searchRadius = 30;

        public override float BaseHp
            => 25;

        [FormerlySerializedAs("speedIncrementForTick")] [SerializeField] private float speedIncrementForFrame = 0.0001f;
        [SerializeField] private float maxSpeed = 5;

        [SerializeField] [CanBeNull] private AudioClip deathSound;
        [SerializeField] [CanBeNull] private AudioClip damageSound;
        [SerializeField] private float generalAudioVolume = 0.1f;

        public override float CurrentHp { get; protected set; }
        [CanBeNull] public override MovingObject CurrentTarget { get; protected set; }
        public override bool IsAlive { get; protected set; }

        public override void TakeDamage(Damage damage)
        {
            if (CurrentHp <= 0)
                return;
            
            CurrentHp -= damage.GetDamageValueExcept(DamageType.Collision);
            CurrentHp -= damage.GetDamageValue(DamageType.Collision) * 0.4f;
            PlaySound(CurrentHp > 0 ? damageSound : deathSound);
        }

        private void UpdateTargetInfo()
        {
            if (CurrentTarget is not null || Player is null || GetDistanceToTarget() > searchRadius)
                return;
            
            CurrentTarget = LayerManager.Instance.CurrentLayer.ContainsObject(gameObject) ? Player.Ship : null;
        }

        private void Start()
        {
            IsAlive = true;
            CurrentHp = BaseHp;
            LayerManager.Instance.AddObject(this.gameObject);
        }

        private void Update()
        {
            EnsureAlive();
            if (!IsAlive)
            {
                EnemyManager.Instance.DespawnEnemy(this);
                return;
            }
            
            UpdateTargetInfo();
            MoveToPlayer();
        }

        private void PlaySound(AudioClip sound)
        {
            if (sound is null)
                return;

            var volume = searchRadius / GetDistanceToTarget() - 1;
            if (volume is null or < 0)
                volume = 0;

            volume = Math.Max((float)volume, 1f) * generalAudioVolume;
            AudioSource.PlayClipAtPoint(sound, transform.position, (float)volume);
        }

        private void MoveToPlayer()
        {
            if (CurrentTarget is null)
                return;
            
            RotateToTarget();
            // var currentRotation = gameObject.transform.rotation.eulerAngles.z * Math.PI / 180;
            // Move(new Vector2((float)Math.Cos(currentRotation),
                // (float)Math.Sin(currentRotation)) * movementSpeed);

            var target = CurrentTarget.transform.position - transform.position;
            Rigidbody.velocity /= 45;
            Move(new Vector2(target.x, target.y));
        }

        private void RotateToTarget()
        {
            if (CurrentTarget is null)
                return;
        }

        private new void FixedUpdate()
        {
            base.FixedUpdate();
            movementSpeed = Math.Min(movementSpeed + speedIncrementForFrame, maxSpeed);
        }
    }
}