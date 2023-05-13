using System;
using JetBrains.Annotations;
using Model.HealthSystem;
using Model.MovingObjects;
using UnityEngine;

namespace Model.Enemies
{
    public abstract class Enemy : MovingObject, IHasHealth
    {
        public abstract float BaseHp { get; }
        public abstract float CurrentHp { get; protected set; }
        
        [CanBeNull] public abstract MovingObject CurrentTarget { get; protected set; }

        [CanBeNull]
        protected static Player Player
            => GameObject.FindWithTag("Player")?.GetComponent<Player>();

        [SerializeField] protected float minDistanceFromPlayerToSpawn = 15;

        public abstract bool IsAlive
        {
            get;
            protected set;
        }

        public virtual void EnsureAlive()
        {
            if (CurrentHp < 0 || IsAlive == false)
                IsAlive = false;
        }

        public abstract void TakeDamage(Damage damage);

        protected float? GetDistanceToTarget()
        {
            if (CurrentTarget is null)
                return null;

            var distanceVector = CurrentTarget.transform.position - gameObject.transform.position;
            return distanceVector.magnitude;
        }
    }
}