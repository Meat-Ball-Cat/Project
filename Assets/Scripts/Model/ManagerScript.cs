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

        [SerializeField] private Light2D _globalLight;

        private Player _player;

        private void Awake()
        {
            Helper.Light ??= shipPartLight;

            var player = new GameObject("Player")
            {
                layer = gameObject.layer
            };
        
            player.AddComponent<Player>();
            _player = player.GetComponent<Player>();

            _globalLight = Instantiate(_globalLight);

            var cameraObject = new GameObject("Main camera");
            var mainCamera = cameraObject.AddComponent<Camera>();
            mainCamera.enabled = true;
            mainCamera.transform.position = new Vector3(0, 0, -25);
            mainCamera.focalLength = 50;
            mainCamera.backgroundColor = Color.black;
            cameraObject.transform.SetParent(player.transform);
        }

        private void UpdateGlobalLight()
        {
            if (_player.CurrentShipDepth >= setMinIntensityAfterDepth)
                _globalLight.intensity = globalLightMinIntensity;
            else
                _globalLight.intensity
                    = (globalLightMaxIntensity - globalLightMinIntensity) *
                    (1 - _player.CurrentShipDepth / setMinIntensityAfterDepth) + globalLightMinIntensity;
        }

        private void Update()
        {
            UpdateGlobalLight();
        }
    }

    public static class Helper
    {
        public static GameObject Light;
    }
}