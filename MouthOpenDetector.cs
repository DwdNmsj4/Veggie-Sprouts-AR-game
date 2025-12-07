using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

public class MouthOpenDetector : MonoBehaviour
{
    [Header("threshold")]
    [Range(0f, 1f)]
    public float threshold = 0.4f;

    [Header("current value")]
    [SerializeField, Range(0f, 1f)]
    private float currentValue = 0f;

    [SerializeField]
    private bool isOpen = false;

    public float CurrentValue => currentValue;
    public bool IsOpen => isOpen;

    ARFace face;
    ARKitFaceSubsystem arkitFaceSubsystem;

    void Awake()
    {
        face = GetComponent<ARFace>();
    }

    void OnEnable()
    {  
        if (face != null)
            face.updated += OnFaceUpdated;

        var faceManager = FindObjectOfType<ARFaceManager>();
        if (faceManager != null)
        {
            arkitFaceSubsystem = faceManager.subsystem as ARKitFaceSubsystem;
        }
    }

    void OnDisable()
    {
        if (face != null)
            face.updated -= OnFaceUpdated;
    }

    void OnFaceUpdated(ARFaceUpdatedEventArgs args)
    {
        if (arkitFaceSubsystem == null)
            return;

        TrackableId id = args.face.trackableId;

        using (NativeArray<ARKitBlendShapeCoefficient> coeffs =
               arkitFaceSubsystem.GetBlendShapeCoefficients(id, Allocator.Temp))
        {
            float jawOpen = 0f;

            for (int i = 0; i < coeffs.Length; i++)
            {
                var c = coeffs[i];
                if (c.blendShapeLocation == ARKitBlendShapeLocation.JawOpen)
                {
                    jawOpen = c.coefficient;  
                    break;
                }
            }

            currentValue = jawOpen;
            isOpen = jawOpen >= threshold;
        }
    }
}