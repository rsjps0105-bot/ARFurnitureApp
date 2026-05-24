using UnityEngine;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public enum PlaneCheckResult
{
    Ok,
    Null,
    NotHorizontal,
    TooSmall,
    DifferentHeight,
    NotConfirmedFloor
}

public class PlaneManager : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARPlaneManager arPlaneManager;

    [Header("UI")]
    [SerializeField] private MessageUIManager messageUIManager;

    [Header("Floor Check")]
    [SerializeField] private float minPlaneSize = 0.5f;
    [SerializeField] private float allowedHeightDifference = 0.1f;

    private bool hasBaseFloor = false;
    private float baseFloorY = 0f;
    private ARPlane confirmedPlane;

    public bool IsFloorConfirmed => hasBaseFloor;

    private void Start()
    {
        ShowMessage("床を映してタップしてください");
    }

    public PlaneCheckResult ConfirmFloor(ARPlane plane, Pose hitPose)
    {
        PlaneCheckResult result = ValidateBasicPlane(plane);

        if (result != PlaneCheckResult.Ok)
        {
            ShowMessage(result);
            return result;
        }

        baseFloorY = hitPose.position.y;
        confirmedPlane = plane;
        hasBaseFloor = true;

        ShowOnlyConfirmedPlane();

        ShowMessage("床を検出しました。家具を配置できます");

        return PlaneCheckResult.Ok;
    }

    public PlaneCheckResult CheckPlane(ARPlane plane, Pose hitPose)
    {
        if (!hasBaseFloor)
        {
            ShowMessage(PlaneCheckResult.NotConfirmedFloor);
            return PlaneCheckResult.NotConfirmedFloor;
        }

        PlaneCheckResult result = ValidateBasicPlane(plane);

        if (result != PlaneCheckResult.Ok)
        {
            ShowMessage(result);
            return result;
        }

        float diff = Mathf.Abs(hitPose.position.y - baseFloorY);

        if (diff > allowedHeightDifference)
        {
            ShowMessage(PlaneCheckResult.DifferentHeight);
            return PlaneCheckResult.DifferentHeight;
        }

        return PlaneCheckResult.Ok;
    }

    private PlaneCheckResult ValidateBasicPlane(ARPlane plane)
    {
        if (plane == null)
            return PlaneCheckResult.Null;

        if (plane.alignment != PlaneAlignment.HorizontalUp)
            return PlaneCheckResult.NotHorizontal;

        if (plane.size.x < minPlaneSize || plane.size.y < minPlaneSize)
            return PlaneCheckResult.TooSmall;

        return PlaneCheckResult.Ok;
    }

    private void ShowOnlyConfirmedPlane()
    {
        if (arPlaneManager == null)
            return;

        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(plane == confirmedPlane);
        }

        // 認定後は新しいPlane検出を止める
        arPlaneManager.enabled = false;
    }

    public void ResetBaseFloor()
    {
        hasBaseFloor = false;
        baseFloorY = 0f;
        confirmedPlane = null;

        if (arPlaneManager != null)
            arPlaneManager.enabled = true;

        if (arSession != null)
            arSession.Reset();

        ShowMessage("床検出をリセットしました。床を映してタップしてください");
    }

    private void ShowAllPlanes()
    {
        if (arPlaneManager == null)
            return;

        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }
    }

    private void ShowMessage(PlaneCheckResult result)
    {
        switch (result)
        {
            case PlaneCheckResult.Null:
                ShowMessage("平面が見つかりません");
                break;

            case PlaneCheckResult.NotHorizontal:
                ShowMessage("床を映してください");
                break;

            case PlaneCheckResult.TooSmall:
                ShowMessage("もう少し広い床を映してください");
                break;

            case PlaneCheckResult.DifferentHeight:
                ShowMessage("認識した床と違う高さには配置できません");
                break;

            case PlaneCheckResult.NotConfirmedFloor:
                ShowMessage("先に床を検出してください");
                break;
        }
    }

    private void ShowMessage(string message)
    {
        if (messageUIManager != null)
        {
            messageUIManager.ShowMessage(message);
        }
    }
}