using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    [SerializeField] 
    private GameObject Light;

    private void Awake()
    {
        Helper.Light ??= Light;

        var player = new GameObject("Player")
        {
            layer = gameObject.layer
        };
        
        player.AddComponent<Player>();

        var cameraObject = new GameObject("Main camera");
        var mainCamera = cameraObject.AddComponent<Camera>();
        mainCamera.enabled = true;
        mainCamera.transform.position = new Vector3(0, 0, -10);
        cameraObject.transform.SetParent(player.transform);
    }

}

public static class Helper
{
    public static GameObject Light;
}
