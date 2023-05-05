using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Renderer))]
internal abstract class ShipPart : MonoBehaviour
{
    private Collider2D _collider;
    private SpriteRenderer _renderer;

    protected readonly Cooldown HitCooldown = new(500);
    
    private int _hitPoint; 
    
    private int HitPoint
    {
        get => _hitPoint;
        set
        {
            if (value < _hitPoint && HitCooldown.CoolingDown) 
                return;
            else if (value < _hitPoint)
                HitCooldown.DelayedStart(10);

            var newHitPoint = Math.Max(value, 0);
            var lastHitPoint = _hitPoint;
            _hitPoint = newHitPoint;

            if (_hitPoint != 0 || lastHitPoint <= 0) 
                return;
            
            gameObject.SetActive(false);
            Died.Invoke(this, null);
        }
    }

    public void Die()
        => HitPoint = 0;
    
    public event EventHandler Died;

    public bool IsAlive
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    public virtual int Width
        => 1;
    public virtual int Height 
        => 1;

    protected void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();

        HitPoint = 25;
        IsAlive = false;
    }

    public readonly HashSet<ShipPart> ConnectedParts = new();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _renderer.color = Color.red;
        HitPoint -= 10;
    }

    private void Update()
    {
        _renderer.color = 
            HitCooldown.CoolingDown 
                ? Color.Lerp(Color.red, Color.white, HitCooldown.ElapsedFrac) 
                : Color.white;
    }
}