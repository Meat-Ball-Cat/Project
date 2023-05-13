using System;
using System.Collections;
using System.Collections.Generic;
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
            CurrentHp = BaseHp;
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

            var target = CurrentTarget.transform.position;
            Turn(0); // Костыль, нельзя пока что поворачивать на точку TODO
            StartRotating();
            Move(target - gameObject.transform.position);
        }

        private Coroutine _lookCoroutine;

        public void StartRotating()
        {
            if (_lookCoroutine != null)
                StopCoroutine(_lookCoroutine);

            _lookCoroutine = StartCoroutine(RotateToTarget());
        }

        private IEnumerator RotateToTarget()
        {
            if (CurrentTarget is null)
                yield break;
            
            var lookRotation = Quaternion.LookRotation(CurrentTarget.transform.position - transform.position);

            float time = 0;

            Quaternion initialRotation = transform.rotation;
            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(initialRotation, lookRotation, time);
                time += Time.deltaTime * turningSpeed;
                yield return null;
            }
        }
    }
}