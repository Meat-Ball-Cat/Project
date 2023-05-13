using System;
using Assets.Scripts.Model.Levels;
using JetBrains.Annotations;
using Model.HealthSystem;
using Model.MovingObjects;
using UnityEngine;

namespace Model.Enemies
{
    public class Chaser : Enemy
    {
        [SerializeField] private float searchRadius = 15;
        [SerializeField] private float rotationSpeedDegreesByTick = 2;

        public override float BaseHp
            => 25;
        
        public override float CurrentHp { get; protected set; }
        [CanBeNull] public override MovingObject CurrentTarget { get; protected set; }
        public override bool IsAlive { get; protected set; }
        
        public override void TakeDamage(Damage damage)
        {
            CurrentHp -= damage.GetDamageValueExcept(DamageType.Collision);
        }

        private void UpdateTargetInfo()
        {
            if (CurrentTarget is not null || Player is null || GetDistanceToTarget() > searchRadius)
                return;

            CurrentTarget = LayerManager.Instance.CurrentLayer.ContainsObject(gameObject) ? Player.Ship : null;
        }

        private void Start()
        {
            CurrentHp = BaseHp;
        }

        private void Update()
        {
            EnsureAlive();
            if (!IsAlive)
                return;
            
            UpdateTargetInfo();
        }

        private void MoveToPlayer()
        {
            if (CurrentTarget is null)
                return;

            var target = CurrentTarget.transform.position;
            Turn(0); // Костыль, нельзя пока что поворачивать на точку TODO
            // TODO поворот на врага и движение
        }
    }
}