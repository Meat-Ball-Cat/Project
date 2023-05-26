using System;
using System.Collections;
using System.Threading;
using Assets.Scripts.Model.Levels;
using Model.HealthSystem;
using UnityEngine;

namespace Model.MovingObjects.Ship.Projectiles
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public abstract class Projectile : MonoBehaviour, IDealsDamage
    {
        [SerializeField] private float baseSpeed = 4;
        [SerializeField] private int enableCollisionAfterMs = 100;

        public abstract Damage Damage { get; }

        public float BaseSpeed
            => baseSpeed;

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!col.gameObject.CompareTag("Enemy"))
                return;

            var damaged = col.gameObject.GetComponent<IHasHealth>();
            damaged?.TakeDamage(Damage);
            
            LayerManager.Instance.CurrentLayer.RemoveObject(gameObject);
            Destroy(gameObject);
        }

        private void Start()
        {
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
            yield break;
        }
    }
}