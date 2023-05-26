using UnityEngine.Rendering.Universal;

namespace Model.MovingObjects.Ship.ShipParts.Parts
{
    internal class Cockpit : ShipPart, ILight, IMoving, ITurning
    {
        private Light2D _light;

        private new void Awake()
        {
            base.Awake();

            _light = GetComponentInChildren<Light2D>();

            IsAlive = true;
        }

        public void SetLightEnabled(bool lightEnabled)
        {
            _light.enabled = lightEnabled;
        }
    
        public float MovementSpeed
            => 3;

        public float TurningSpeed
            => 1;
    }
}