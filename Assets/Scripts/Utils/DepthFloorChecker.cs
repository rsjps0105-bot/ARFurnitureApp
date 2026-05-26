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

        // Depth画像を取得
        if (!occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            Debug.LogWarning("Depth画像を取得できませんでした");
            return true;
        }

        // Depth画像を使用後は必ずDisposeする必要がある(重い)ため、usingブロックで囲む
        using (image)
        {
            // 画面座標をDepth画像の座標に変換
            Vector2 imagePoint = ScreenToDepthImagePoint(image, screenPoint);

            int centerX = Mathf.RoundToInt(imagePoint.x);
            int centerY = Mathf.RoundToInt(imagePoint.y);

            float minDepth = float.MaxValue;
            float maxDepth = float.MinValue;
            int validCount = 0;

            // 周辺Depthをサンプリングして、最大と最小の深度を求める
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

    // Unity画面座標(左下原点)を、Depth画像座標(左上原点)に変換している
    private Vector2 ScreenToDepthImagePoint(XRCpuImage image, Vector2 screenPoint)
    {
        float x = screenPoint.x / Screen.width * image.width;

        // Unityの画面座標は左下原点、Depth画像は上基準になることがあるので反転
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

        /*
            Depth画像の1ピクセルは 16bit(2バイト) で保存されている。

            plane.data は byte 配列なので、
            1回で取得できるのは 8bit (=1バイト) だけ。

            そのため、

            index     → 下位8bit
            index + 1 → 上位8bit

            の2つを結合して、
            1つの16bit深度値を作る必要がある。

            plane.data[index + 1] << 8
            は、上位8bitを左へ8bitずらして
            上位バイトの位置へ移動している。

            例:

            下位 = 0x34
            上位 = 0x12

            0x12 << 8
                ↓
            0x1200

            その後、| (OR演算) で結合する。

            0x1200 | 0x34
                ↓
            0x1234

            最終的に 0x1234 = 4660(mm)
    
            つまり約 4.66m の深度値になる。
        */
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