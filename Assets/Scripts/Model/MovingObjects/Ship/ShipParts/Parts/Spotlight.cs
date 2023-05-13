using UnityEngine.Rendering.Universal;

namespace Model.MovingObjects.Ship.ShipParts.Parts
{
    internal class Spotlight : ShipPart, ILight
    {
        private Light2D _light;

        private new void Awake()
        {
            base.Awake();

            _light = GetComponentInChildren<Light2D>();
        }

        public void SetLightEnabled(bool enabledLight)
        {
            _light.enabled = enabledLight;
        }
    }
}