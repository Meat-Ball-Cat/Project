using System;
using System.Collections.Generic;
using Model.HealthSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.MovingObjects.Ship.ShipParts
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Renderer))]
    public abstract class ShipPart : MonoBehaviour, IHasHealth
    {
        private Collider2D _collider;
        private SpriteRenderer _renderer;

        protected readonly Cooldown HitCooldown = new(500);

        [SerializeField] private float baseHp = 25;

        public float BaseHp
            => baseHp;
        
        private float _currentHp; 
        
        
        public float CurrentHp
        {
            get
                => _currentHp;
            private set
            {
                if (value > 0 && value < _currentHp && HitCooldown.CoolingDown) 
                    return;
                else if (value < _currentHp)
                    HitCooldown.DelayedStart(10);

                var newHitPoint = Math.Max(value, 0f);
                var lastHitPoint = _currentHp;
                _currentHp = newHitPoint;

                if (_currentHp != 0 || lastHitPoint <= 0) 
                    return;
            
                gameObject.SetActive(false);
                Died.Invoke(this, null);
            }
        }

        public void Die()
            => CurrentHp = 0;
    
        public event EventHandler Died;

        public bool IsAlive
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public void EnsureAlive()
        {
            return;
        }

        public void TakeDamage(Damage damage)
        {
            _currentHp -= damage.GetDamageValueExcept();
        }


        public virtual int Width
            => 1;

        public virtual int Height 
            => 1;

        protected void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponent<SpriteRenderer>();

            CurrentHp = baseHp;
            IsAlive = false;
        }

        public readonly HashSet<ShipPart> ConnectedParts = new();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"layer {gameObject.layer} - layer {collision.gameObject.layer}");
            _renderer.color = Color.red;
            CurrentHp -= 10;
        }

        private void Update()
        {
            _renderer.color = 
                HitCooldown.CoolingDown 
                    ? Color.Lerp(Color.red, Color.white, HitCooldown.ElapsedFrac) 
                    : Color.white;
        }
    }
}