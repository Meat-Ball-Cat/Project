using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Model
{
    public class ManagerScript : MonoBehaviour
    {
        [FormerlySerializedAs("Light")] [SerializeField]
        private GameObject shipPartLight;

        [SerializeField] private float globalLightMaxIntensity = 1, 
            globalLightMinIntensity = 0.05f,
            setMinIntensityAfterDepth = 2000f;

        [SerializeField] private Color globalLightColor = Color.white;

        private Player _player;
        private Light2D _globalLight;
        
        private void Awake()
        {
            Helper.Light ??= shipPartLight;

            var player = new GameObject("Player")
            {
                layer = gameObject.layer
            };
        
            player.AddComponent<Player>();
            _player = player.GetComponent<Player>();

            SetupGlobalLight();

            var cameraObject = new GameObject("Main camera");
            var mainCamera = cameraObject.AddComponent<Camera>();
            mainCamera.enabled = true;
            mainCamera.transform.position = new Vector3(0, 0, -10);
            cameraObject.transform.SetParent(player.transform);
        }

        private void SetupGlobalLight()
        {
            _globalLight = gameObject.AddComponent<Light2D>();
            _globalLight.lightType = Light2D.LightType.Global;
            _globalLight.color = globalLightColor;
            _globalLight.intensity = globalLightMaxIntensity;
        }

        private void Update()
        {
            if (_player.CurrentShipDepth >= setMinIntensityAfterDepth)
                _globalLight.intensity = globalLightMinIntensity;
            else
                _globalLight.intensity
                    = (globalLightMaxIntensity - globalLightMinIntensity) *
                    (1 - _player.CurrentShipDepth / setMinIntensityAfterDepth) + globalLightMinIntensity;
        }
    }

    public static class Helper
    {
        public static GameObject Light;
    }
}