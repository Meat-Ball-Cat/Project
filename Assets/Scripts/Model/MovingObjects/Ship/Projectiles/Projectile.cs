using System;
using System.Collections;
using System.Threading;
using Assets.Scripts.Model.Levels;
using JetBrains.Annotations;
using Model.Enemies;
using Model.HealthSystem;
using UnityEngine;

namespace Model.MovingObjects.Ship.Projectiles
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour, IDealsDamage
    {
        [SerializeField] private float baseSpeed = 4;

        [SerializeField] private float explosionDamage = 0;
        [SerializeField] private float collisionDamage = 0;

        [SerializeField] private float despawnDistance = 80;

        [SerializeField] [CanBeNull] private GameObject hitEffectPrefab;

        [SerializeField] [CanBeNull] private AudioClip hitSound;
        [SerializeField] private float generalAudioVolume = 0.2f;
        
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
            if (hitEffectPrefab is not null)
                Instantiate(hitEffectPrefab, transform.position, new Quaternion());
            
            PlaySound(hitSound);
            Destroy(gameObject);
        }

        private void Start()
        {
            _spawnedOn = LayerManager.Instance.CurrentLayer;
            LayerManager.Instance.AddObject(gameObject);
        }

        private void Update()
        {
            if (GetDistanceToPlayer() > despawnDistance || LayerManager.Instance.CurrentLayer != _spawnedOn)
                Destroy(this);
        }
        
        private void PlaySound(AudioClip sound)
        {
            if (sound is null)
                return;

            var volume = despawnDistance / GetDistanceToPlayer() - 1;
            if (volume is null or < 0)
                volume = 0;

            volume = Math.Max((float)volume, 1f) * generalAudioVolume; 
            AudioSource.PlayClipAtPoint(sound, transform.position, (float)volume);
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