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
            _hitPoint = Math.Max(value, 0);
            if (_hitPoint == 0)
            {
                Died.Invoke(this, null);
                gameObject.SetActive(false);
            }
        }
    }

    public void Die() => HitPoint = 0;
    public event EventHandler Died;
    public bool Alive => gameObject.activeSelf;

    public virtual int Width => 1;
    public virtual int Height => 1;

    protected void Awake()
    {
        _collider = GetComponent<Collider2D>();

        _renderer = GetComponent<SpriteRenderer>();

        HitPoint = 25;
    }

    public readonly HashSet<PartShip> ConnectedParts = new ();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _renderer.color = Color.red;
        HitPoint -= 10;
        Debug.Log(HitPoint);
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