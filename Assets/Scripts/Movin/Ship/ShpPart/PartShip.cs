using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Renderer))]
internal abstract class PartShip : MonoBehaviour
{
    private readonly float _whiteningTime = 5f;
    private Collider2D _collider;
    private SpriteRenderer _renderer;
    private float _timePassed;

    private int _hitPoint; 
    private int HitPoint
    {
        get => _hitPoint;
        set
        {

            var newHitPoint = Math.Max(value, 0);
            var lastHitPoint = _hitPoint;
            _hitPoint = newHitPoint;

            if (_hitPoint != 0 || lastHitPoint <= 0) return;
            gameObject.SetActive(false);
            Died.Invoke(this, null);
        }
    }

    public void Die() => HitPoint = 0;
    public event EventHandler Died;

    public bool Alive
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    public virtual int Width => 1;
    public virtual int Height => 1;

    protected void Awake()
    {
        _collider = GetComponent<Collider2D>();

        _renderer = GetComponent<SpriteRenderer>();

        HitPoint = 25;

        Alive = false;
    }

    public readonly HashSet<PartShip> ConnectedParts = new ();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _renderer.color = Color.red;
        HitPoint -= 10;
        _timePassed = 0;
    }

    private void Update()
    {
        if (_timePassed < _whiteningTime)
        {
            _timePassed += Time.deltaTime;
            _renderer.color = _timePassed < _whiteningTime
                ? Color.Lerp(Color.red, Color.white, _timePassed / _whiteningTime)
                : Color.white;
        }
    }
}