using UnityEngine;

[RequireComponent(typeof(Collider2D))]
internal abstract class PartShip : MonoBehaviour
{
    private readonly float _whiteningTime = 5f;
    private Collider2D _collider;
    private SpriteRenderer _renderer;
    private float _timePassed;

    protected void Awake()
    {
        _collider = GetComponent<Collider2D>();

        _renderer = GetComponent<SpriteRenderer>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        _renderer.color = Color.red;
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