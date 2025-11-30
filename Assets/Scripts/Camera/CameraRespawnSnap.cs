using UnityEngine;
using Unity.Cinemachine;

public class CameraRespawnSnap : MonoBehaviour
{
    public static CameraRespawnSnap Instance { get; private set; }

    private CinemachineCamera cinemachineCamera;
    private CinemachineFollow cinemachineFollow;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        cinemachineCamera = GetComponent<CinemachineCamera>();
        cinemachineFollow = GetComponent<CinemachineFollow>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SnapToTarget()
    {
        if (cinemachineCamera != null && cinemachineCamera.Follow != null)
        {
            cinemachineCamera.PreviousStateIsValid = false;
            
            cinemachineCamera.ForceCameraPosition(
                cinemachineCamera.Follow.position, 
                cinemachineCamera.Follow.rotation
            );
            
            if (cinemachineFollow != null)
            {
                cinemachineFollow.ForceCameraPosition(
                    cinemachineCamera.Follow.position, 
                    cinemachineCamera.Follow.rotation
                );
            }
        }
    }
}
