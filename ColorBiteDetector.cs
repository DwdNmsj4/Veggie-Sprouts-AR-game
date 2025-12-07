using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public enum VegType
{
    None,
    Green,   
    Red,     
    Orange,  
    Purple   
}
public class ColorBiteDetector : MonoBehaviour
{
    [Header("all")]
    public MouthOpenDetector mouthDetector;   
    public Transform mouthAnchor;            
    public Camera arCamera;                  
    public ARCameraManager arCameraManager;  
    public FarmGameManager farmManager;      

    [Header("coloe detection")]
    [Range(0.0f, 1.0f)]
    [Tooltip("sample viewport radium")]
    public float sampleViewportRadius = 0.08f;

    [Range(0.0f, 1.0f)]
    [Tooltip("color ratio threshold")]
    public float colorRatioThreshold = 0.4f;

    [Tooltip("min brightness")]
    public float minBrightness = 0.25f;

    [Tooltip("max brightness")]
    public float maxBrightness = 0.85f;

    private bool biteConsumedThisOpen = false;

    private NativeArray<byte> _imageBuffer;

    [HideInInspector]
    public VegType lastDetectedVeg = VegType.None;

    private bool colorHintShown = false;


    void OnDestroy()
    {
        if (_imageBuffer.IsCreated)
            _imageBuffer.Dispose();
    }

    void Update()
    {
        if (mouthDetector == null || mouthAnchor == null || farmManager == null || arCamera == null || arCameraManager == null)
            return;
     
        if (!mouthDetector.IsOpen)
        {
            biteConsumedThisOpen = false;
            return;
        }

        if (biteConsumedThisOpen)
            return;

        VegType veg = DetectVegNearMouth();

        if (veg != VegType.None)
        {
            Debug.Log($"[ColorBiteDetector] Bite success, detected veg = {veg}");

            VegType previousVeg = lastDetectedVeg;

            biteConsumedThisOpen = true;
            lastDetectedVeg = veg;
  
            farmManager.OnBite(veg);
        }

    }

    private VegType DetectVegNearMouth()
    {
        Vector3 screenPos = arCamera.WorldToScreenPoint(mouthAnchor.position);

        if (screenPos.z < 0 ||
            screenPos.x < 0 || screenPos.x > Screen.width ||
            screenPos.y < 0 || screenPos.y > Screen.height)
        {
            return VegType.None;
        }

        if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            return VegType.None;
        }

        using (image)
        {
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, image.width, image.height),
                outputDimensions = new Vector2Int(image.width, image.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorX 
            };

            int size = image.GetConvertedDataSize(conversionParams);
            if (!_imageBuffer.IsCreated || _imageBuffer.Length < size)
            {
                if (_imageBuffer.IsCreated)
                    _imageBuffer.Dispose();

                _imageBuffer = new NativeArray<byte>(size, Allocator.Persistent);
            }

            image.Convert(conversionParams, _imageBuffer);

            int imgWidth = image.width;
            int imgHeight = image.height;
            float u = screenPos.x / Screen.width;
            float v = screenPos.y / Screen.height;

            int centerX = Mathf.Clamp(Mathf.RoundToInt(u * imgWidth), 0, imgWidth - 1);
            int centerY = Mathf.Clamp(Mathf.RoundToInt(v * imgHeight), 0, imgHeight - 1);

            float shortSide = Mathf.Min(imgWidth, imgHeight);
            int radiusPixels = Mathf.Max(2, Mathf.RoundToInt(sampleViewportRadius * shortSide));

            int xMin = Mathf.Max(centerX - radiusPixels, 0);
            int xMax = Mathf.Min(centerX + radiusPixels, imgWidth - 1);
            int yMin = Mathf.Max(centerY - radiusPixels, 0);
            int yMax = Mathf.Min(centerY + radiusPixels, imgHeight - 1);

            int totalCount = 0;
            int redCount = 0;
            int orangeCount = 0;
            int greenCount = 0;
            int purpleCount = 0;

            int step = Mathf.Max(1, radiusPixels / 5);

            for (int y = yMin; y <= yMax; y += step)
            {
                for (int x = xMin; x <= xMax; x += step)
                {
                    int index = (y * imgWidth + x) * 4; 

                    float rf = _imageBuffer[index + 0] / 255f;
                    float gf = _imageBuffer[index + 1] / 255f;
                    float bf = _imageBuffer[index + 2] / 255f;

                   
                    Color c = new Color(rf, gf, bf);
                    Color.RGBToHSV(c, out float h, out float s, out float value);

                  
                    if (value < minBrightness || value > maxBrightness)
                        continue;

                    totalCount++;

                    if (IsRed(h))
                        redCount++;
                    else if (IsOrange(h))
                        orangeCount++;
                    else if (IsGreen(h))
                        greenCount++;
                    else if (IsPurple(h))
                        purpleCount++;
                }
            }

            if (totalCount == 0)
                return VegType.None;

         
            float redRatio = (float)redCount / totalCount;
            float orangeRatio = (float)orangeCount / totalCount;
            float greenRatio = (float)greenCount / totalCount;
            float purpleRatio = (float)purpleCount / totalCount;

            Debug.Log($"[ColorBiteDetector] ratio R:{redRatio:F2} O:{orangeRatio:F2} G:{greenRatio:F2} P:{purpleRatio:F2}");

            float bestRatio = 0f;
            VegType best = VegType.None;

            void TryUpdate(float ratio, VegType veg)
            {
                if (ratio > bestRatio)
                {
                    bestRatio = ratio;
                    best = veg;
                }
            }

            TryUpdate(redRatio, VegType.Red);
            TryUpdate(orangeRatio, VegType.Orange);
            TryUpdate(greenRatio, VegType.Green);
            TryUpdate(purpleRatio, VegType.Purple);

            if (best != VegType.None && bestRatio >= colorRatioThreshold)
                return best;

            return VegType.None;
        }
    }
    private bool IsRed(float h)
    {
        return (h >= 0f && h <= 0.03f) || (h >= 0.95f && h <= 1f);
    }

    private bool IsOrange(float h)
    {
        return (h >= 0.05f && h <= 0.12f);
    }

    private bool IsGreen(float h)
    {
        return (h >= 0.20f && h <= 0.45f);
    }

    private bool IsPurple(float h)
    {
        return (h >= 0.72f && h <= 0.85f);

    }

}
