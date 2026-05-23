using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Камера, которая должна включиться в этой зоне")]
    public CinemachineCamera zoneCamera;

    [Header("Это стартовая зона игры?")]
    public bool isStartingZone = false; 


    private void Start()
    {
        if (isStartingZone && zoneCamera != null)
        {
            ActivateCamera();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateCamera();
        }
    }

    private void ActivateCamera()
    {
        var allCameras = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        foreach (var cam in allCameras)
        {
            cam.Priority = 10;
        }

        zoneCamera.Priority = 15;
    }
}