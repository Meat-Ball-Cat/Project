using System;
using Assets.Scripts.Model.Levels;
using JetBrains.Annotations;
using Model.HealthSystem;
using Model.MovingObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Model.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Chaser : Enemy
    {
        [SerializeField] private float searchRadius = 30;

        public override float BaseHp
            => 25;

        [FormerlySerializedAs("speedIncrementForTick")] [SerializeField] private float speedIncrementForFrame = 0.0001f;
        [SerializeField] private float maxSpeed = 5;

        [SerializeField] [CanBeNull] private AudioClip deathSound;
        [SerializeField] private float generalAudioVolume = 0.1f;

        [SerializeField] private float randomRotationSpeed = 0f;
        [SerializeField] private int randomRotationCooldownMs = 350;

        private Random _random = new();
        private Cooldown _rotationCooldown;

        private Rigidbody2D _rb;

        public override float CurrentHp { get; protected set; }
        [CanBeNull] public override MovingObject CurrentTarget { get; protected set; }
        public override bool IsAlive { get; protected set; }

        public override void TakeDamage(Damage damage)
        {
            if (CurrentHp <= 0)
                return;
            
            CurrentHp -= damage.GetDamageValueExcept(DamageType.Collision);
            CurrentHp -= damage.GetDamageValue(DamageType.Collision) * 0.4f;
            if (CurrentHp < 0)
                PlaySound(deathSound);
        }

        private void UpdateTargetInfo()
        {
            if (CurrentTarget is not null || Player is null || GetDistanceToTarget() > searchRadius)
                return;
            
            Debug.Log($"Acquired target for {this}");
            CurrentTarget = LayerManager.Instance.CurrentLayer.ContainsObject(gameObject) ? Player.Ship : null;
        }

        private void Start()
        {
            IsAlive = true;
            CurrentHp = BaseHp;
            LayerManager.Instance.AddObject(this.gameObject);

            _rb = GetComponent<Rigidbody2D>();
            _rotationCooldown = new(randomRotationCooldownMs);
        }

        private void Update()
        {
            EnsureAlive();
            if (!IsAlive)
            {
                EnemyManager.Instance.DespawnEnemy(this);
                return;
            }
            
            RandomRotate();
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
            
            // Rigidbody.velocity = target.normalized * Math.Min(Rigidbody.velocity.magnitude, movementSpeed);
            Move(new Vector2(target.x, target.y));
        }

        private void RotateToTarget()
        {
            if (CurrentTarget is null)
                return;
        }

        private void RandomRotate()
        {
            if (_rotationCooldown.CoolingDown)
                return;
            
            _rb.AddTorque(NextFloat() * randomRotationSpeed);
            _rotationCooldown.Start();
        }

        private float NextFloat()
        {
            var buffer = new byte[4];
            _random.NextBytes(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        private new void FixedUpdate()
        {
            base.FixedUpdate();
            movementSpeed = Math.Min(movementSpeed + speedIncrementForFrame, maxSpeed);
        }
    }
}