using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    public CinemachineVirtualCamera startCamera;
    public CinemachineVirtualCamera followCamera;

    [SerializeField]
    Vector3 PlayerOffset = new Vector3(0, 10f, -10);

    int cameraIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void SetPriority(Transform target, int cameraIndex )
    {
        if (cameraIndex == 0)
        {
            startCamera.Follow = target;
            followCamera.LookAt = target;
        }
        if (cameraIndex == 1)
        {
            transform.position = target.position + PlayerOffset;
            followCamera.Follow = target;
            followCamera.LookAt = target;
        }
      
    }
}
