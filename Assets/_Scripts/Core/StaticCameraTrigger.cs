using Unity.Cinemachine;
using Unity.Cinemachine;
using UnityEngine;

public class StaticCameraTrigger : MonoBehaviour
{
    [Header("Камера этой зоны")]
    public CinemachineCamera zoneCamera;

    // Срабатывает, когда игрок входит в эту зону
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (zoneCamera != null)
            {
                var allCameras = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
                foreach (var cam in allCameras)
                {
                    cam.Priority = 10;
                }

                zoneCamera.Priority = 20;
            }
        }
    }
}