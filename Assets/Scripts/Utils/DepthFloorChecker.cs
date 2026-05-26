using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DepthFloorChecker : MonoBehaviour
{
    [SerializeField] private AROcclusionManager occlusionManager;

    [Header("Depth Check")]
    [SerializeField] private int sampleRadius = 4;
    [SerializeField] private float maxDepthDifference = 0.15f;
    [SerializeField] private int minValidSamples = 8;

    public bool IsFlatAroundScreenPoint(Vector2 screenPoint)
    {
        if (occlusionManager == null)
        {
            Debug.LogWarning("AROcclusionManager が設定されていません");
            return true;
        }

        if (!occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            Debug.LogWarning("Depth画像を取得できませんでした");
            return true;
        }

        using (image)
        {
            Vector2 imagePoint = ScreenToDepthImagePoint(image, screenPoint);

            int centerX = Mathf.RoundToInt(imagePoint.x);
            int centerY = Mathf.RoundToInt(imagePoint.y);

            float minDepth = float.MaxValue;
            float maxDepth = float.MinValue;
            int validCount = 0;

            for (int y = -sampleRadius; y <= sampleRadius; y++)
            {
                for (int x = -sampleRadius; x <= sampleRadius; x++)
                {
                    int px = centerX + x;
                    int py = centerY + y;

                    if (TryGetDepthMeters(image, px, py, out float depth))
                    {
                        minDepth = Mathf.Min(minDepth, depth);
                        maxDepth = Mathf.Max(maxDepth, depth);
                        validCount++;
                    }
                }
            }

            if (validCount < minValidSamples)
            {
                Debug.Log($"深度サンプル不足: {validCount}");
                return true;
            }

            float diff = maxDepth - minDepth;
            Debug.Log($"Depth Flat Check diff={diff:F2}, samples={validCount}");

            return diff <= maxDepthDifference;
        }
    }

    private Vector2 ScreenToDepthImagePoint(XRCpuImage image, Vector2 screenPoint)
    {
        float x = screenPoint.x / Screen.width * image.width;

        // Unityの画面座標は左下原点、Depth画像は上基準になることがあるので反転
        float y = (1f - screenPoint.y / Screen.height) * image.height;

        return new Vector2(x, y);
    }

    private bool TryGetDepthMeters(XRCpuImage image, int x, int y, out float depthMeters)
    {
        depthMeters = 0f;

        if (x < 0 || x >= image.width || y < 0 || y >= image.height)
        {
            return false;
        }

        XRCpuImage.Plane plane = image.GetPlane(0);

        int index = y * plane.rowStride + x * plane.pixelStride;

        if (index < 0 || index + 1 >= plane.data.Length)
        {
            return false;
        }

        ushort depthMillimeters =
            (ushort)(plane.data[index] | (plane.data[index + 1] << 8));

        if (depthMillimeters == 0)
        {
            return false;
        }

        depthMeters = depthMillimeters / 1000f;
        return true;
    }
}