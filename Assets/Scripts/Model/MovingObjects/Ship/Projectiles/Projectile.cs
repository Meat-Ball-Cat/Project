using System;
using System.Collections;
using System.Threading;
using Assets.Scripts.Model.Levels;
using Model.Enemies;
using Model.HealthSystem;
using UnityEngine;

namespace Model.MovingObjects.Ship.Projectiles
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour, IDealsDamage
    {
        [SerializeField] private float baseSpeed = 4;
        [SerializeField] private int enableCollisionAfterMs = 200;

        [SerializeField] private float explosionDamage = 0;
        [SerializeField] private float collisionDamage = 0;

        [SerializeField] private float despawnDistance = 250;

        private Layer _spawnedOn;

        public virtual Damage Damage
            => new((DamageType.Explosion, explosionDamage), (DamageType.Collision, collisionDamage));

        public float BaseSpeed
            => baseSpeed;

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Projectile"))
                return;

            var damaged = col.gameObject.GetComponent<IHasHealth>();
            damaged?.TakeDamage(Damage);
            
            LayerManager.Instance.CurrentLayer.RemoveObject(gameObject);
            Destroy(gameObject);
        }

        private void Start()
        {
            _spawnedOn = LayerManager.Instance.CurrentLayer;
            
            GetComponent<Collider2D>().enabled = false;
            LayerManager.Instance.AddObject(gameObject);
            StartCoroutine(EnableCollider());
        }

        public IEnumerator EnableCollider()
        {
            var activeCooldown = new Cooldown(enableCollisionAfterMs);
            activeCooldown.Start();
            while (activeCooldown.CoolingDown)
                yield return null;
        
            GetComponent<Collider2D>().enabled = true;
        }

        private void Update()
        {
            if (GetDistanceToPlayer() > despawnDistance || LayerManager.Instance.CurrentLayer != _spawnedOn)
                Destroy(this);
        }

        private float? GetDistanceToPlayer()
        {
            if (Enemy.Player is null)
                return null;

            var distanceVector = Enemy.Player.transform.position - gameObject.transform.position;
            return distanceVector.magnitude;
        }
    }
}