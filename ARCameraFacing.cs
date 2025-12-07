using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARCameraManager))]
public class ARCameraFacing : MonoBehaviour
{
    void OnEnable()
    {
        var cam = GetComponent<ARCameraManager>();
        cam.requestedFacingDirection = CameraFacingDirection.User;
    }
}