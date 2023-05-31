using Assets.Scripts.Model.Levels;
using JetBrains.Annotations;
using Model.HealthSystem;
using Model.MovingObjects;
using UnityEngine;

namespace Model.Enemies
{
    public class Chaser : Enemy
    {
        [SerializeField] private float searchRadius = 30;

        public override float BaseHp
            => 25;
        
        public override float CurrentHp { get; protected set; }
        [CanBeNull] public override MovingObject CurrentTarget { get; protected set; }
        public override bool IsAlive { get; protected set; }

        public override void TakeDamage(Damage damage)
        {
            CurrentHp -= damage.GetDamageValueExcept(DamageType.Collision);
            CurrentHp -= damage.GetDamageValue(DamageType.Collision) * 0.4f;
            EnsureAlive();
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

        private void MoveToPlayer()
        {
            if (CurrentTarget is null)
                return;
            
            // RotateToTarget();
            // var currentRotation = gameObject.transform.rotation.eulerAngles.z * Math.PI / 180;
            // Move(new Vector2((float)Math.Cos(currentRotation),
                // (float)Math.Sin(currentRotation)) * movementSpeed);

            var target = CurrentTarget.transform.position - transform.position;
            Rigidbody.velocity /= 45;
            Move(new Vector2(target.x, target.y));
        }

        private Coroutine _lookCoroutine;

        private void RotateToTarget()
        {
            if (CurrentTarget is null)
                return;

            var lookAt = CurrentTarget.transform.position;
            gameObject.transform.LookAt(lookAt);
        }
    }
}