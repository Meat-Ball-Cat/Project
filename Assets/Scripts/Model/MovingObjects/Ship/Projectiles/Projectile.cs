using System;
using Assets.Scripts.Model.Levels;
using Model.HealthSystem;
using UnityEngine;

namespace Model.MovingObjects.Ship.Projectiles
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = 4;
        public abstract Damage Damage { get; }

        public float BaseSpeed
            => baseSpeed;

        private void OnCollisionEnter2D(Collision2D col)
        {
            LayerManager.Instance.CurrentLayer.RemoveObject(gameObject);
            Destroy(gameObject);
        }

        private void Awake()
        {
            LayerManager.Instance.AddObject(gameObject);
        }
    }
}