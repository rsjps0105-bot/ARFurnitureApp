using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DepthPlacementValidator : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private AROcclusionManager occlusionManager;

    [Header("Depth Check")]
    [SerializeField] private float depthTolerance = 0.05f;
    [SerializeField] private int sampleRadius = 4;
    [SerializeField] private int minValidSamples = 5;

    public bool CanPlaceAtWorldPosition(Vector3 worldPosition)
    {
        if (arCamera == null || occlusionManager == null)
        {
            Debug.LogWarning("Camera または AROcclusionManager が設定されていません");
            return true;
        }

        // ワールド位置をスクリーン座標に変換
        Vector3 screenPoint = arCamera.WorldToScreenPoint(worldPosition);

        if (screenPoint.z <= 0)
        {
            return false;
        }

        // Depth画像を取得
        if (!occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            Debug.LogWarning("Depth画像を取得できませんでした");
            return true;
        }

        using (image)
        {
            // Depth画像からスクリーンポイント周辺の平均深度を取得
            if (!TryGetAverageDepth(image, screenPoint, out float realDepth))
            {
                Debug.Log("有効なDepthサンプルが足りません");
                return true;
            }

            float objectDepth = screenPoint.z;

            // 家具予定位置が、現実の表面より奥にあるかどうかをチェック
            if (objectDepth > realDepth + depthTolerance)
            {
                Debug.Log($"Depth配置NG object={objectDepth:F2}m real={realDepth:F2}m");
                return false;
            }

            Debug.Log($"Depth配置OK object={objectDepth:F2}m real={realDepth:F2}m");
            return true;
        }
    }

    // Depth画像からスクリーンポイント周辺の平均深度を取得
    private bool TryGetAverageDepth(XRCpuImage image, Vector3 screenPoint, out float averageDepth)
    {
        averageDepth = 0f;

        Vector2 imagePoint = ScreenToDepthImagePoint(image, screenPoint);

        int centerX = Mathf.RoundToInt(imagePoint.x);
        int centerY = Mathf.RoundToInt(imagePoint.y);

        float total = 0f;
        int validCount = 0;

        // 周辺Depthをサンプリングして、平均深度を求める
        for (int y = -sampleRadius; y <= sampleRadius; y++)
        {
            for (int x = -sampleRadius; x <= sampleRadius; x++)
            {
                int px = centerX + x;
                int py = centerY + y;

                if (TryGetDepthMeters(image, px, py, out float depth))
                {
                    total += depth;
                    validCount++;
                }
            }
        }

        if (validCount < minValidSamples)
        {
            return false;
        }

        averageDepth = total / validCount;
        return true;
    }

    // Unity画面座標(左下原点)を、Depth画像座標(左上原点)に変換している
    private Vector2 ScreenToDepthImagePoint(XRCpuImage image, Vector2 screenPoint)
    {
        float x = screenPoint.x / Screen.width * image.width;
        float y = (1f - screenPoint.y / Screen.height) * image.height;

        return new Vector2(x, y);
    }

    // Depth画像から特定のピクセルの深度をメートル単位で取得
    private bool TryGetDepthMeters(XRCpuImage image, int x, int y, out float depthMeters)
    {
        depthMeters = 0f;

        if (x < 0 || x >= image.width || y < 0 || y >= image.height)
        {
            return false;
        }

        XRCpuImage.Plane plane = image.GetPlane(0);

        // (x, y) のDepth値が、plane.data 配列の何番目にあるかを計算
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