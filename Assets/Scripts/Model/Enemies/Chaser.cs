using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.SymbolStore;
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
            IsAlive = true;
            CurrentHp = BaseHp;
            LayerManager.Instance.AddObject(this.gameObject);
        }

        private void Update()
        {
            EnsureAlive();
            if (!IsAlive)
                return;
            
            UpdateTargetInfo();
            MoveToPlayer();
        }

        private void MoveToPlayer()
        {
            if (CurrentTarget is null)
                return;
            
            // RotateToTarget();
            var target = CurrentTarget.transform.position;
            Move((target - gameObject.transform.position).normalized * movementSpeed);
        }

        private Coroutine _lookCoroutine;

        private void RotateToTarget()
        {
            if (CurrentTarget is null)
                return;
            
            gameObject.transform.Rotate(CurrentTarget.transform.position);
        }
    }
}