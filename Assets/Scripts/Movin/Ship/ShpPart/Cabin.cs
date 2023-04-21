using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

internal class Cabin : PartShip, ILight, IMoving, ITurning
{
    private Light2D _light;

    private new void Awake()
    {
        base.Awake();

        _light = Instantiate(Helper.Light).GetComponent<Light2D>();
        _light.pointLightOuterRadius = 5;
        _light.intensity = 3;
        _light.transform.SetParent(gameObject.transform);

        Alive = true;
    }

    public void SetLightEnabled(bool lightEnabled) => _light.gameObject.SetActive(lightEnabled);
    public float MovementSpeed => 3;
    public float TurningSpeed => 1;
}