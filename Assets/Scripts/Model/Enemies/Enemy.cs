using System;
using Assets.Scripts.Model.Levels;
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

        [SerializeField] protected float minDistanceFromPlayerToSpawn = 60;

        public float MinDistanceFromPlayerToSpawn
            => minDistanceFromPlayerToSpawn;

        public int MaxNumberOnMap
            => maxNumberOnMap;

        public float DistanceToDespawn
            => distanceToDespawn;

        [SerializeField] protected int maxNumberOnMap = 40;
        [SerializeField] protected float distanceToDespawn = 100;

        public abstract bool IsAlive
        {
            get;
            protected set;
        }

        public virtual void EnsureAlive()
        {
            if (!(CurrentHp < 0) && IsAlive != false)
                return;
            
            IsAlive = false;
            //TODO удалять со слоя после смерти
        }

        public abstract void TakeDamage(Damage damage);

        public float? GetDistanceToTarget()
        {
            if (CurrentTarget is null)
                return null;

            var distanceVector = CurrentTarget.transform.position - gameObject.transform.position;
            return distanceVector.magnitude;
        }
    }
}