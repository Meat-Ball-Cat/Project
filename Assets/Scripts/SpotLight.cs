
    using UnityEngine.Rendering.Universal;

    internal class SpotLight : PartShip, ILight
    { 
        private Light2D _light;

        private new void Awake()
        {
            base.Awake();

            _light = Instantiate(Helper.Light).GetComponent<Light2D>();
            _light.transform.SetParent(gameObject.transform);
        }

        public void SetLightEnabled(bool enabledLight)
        {
            _light.enabled = enabledLight;
        }
    }

