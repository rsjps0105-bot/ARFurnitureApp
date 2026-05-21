using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public enum PlaneCheckResult
{
    Ok,
    Null,
    NotHorizontal,
    TooSmall,
    DifferentHeight
}

public class PlaneManager : MonoBehaviour
{
    [SerializeField] private float minPlaneSize = 0.5f;
    [SerializeField] private float allowedHeightDifference = 0.1f;

    private bool hasBaseFloor = false;
    private float baseFloorY;

    public PlaneCheckResult CheckPlane(ARPlane plane, Pose hitPose)
    {
        if (plane == null)
            return PlaneCheckResult.Null;

        if (plane.alignment != PlaneAlignment.HorizontalUp)
            return PlaneCheckResult.NotHorizontal;

        if (plane.size.x < minPlaneSize || plane.size.y < minPlaneSize)
            return PlaneCheckResult.TooSmall;

        float hitY = hitPose.position.y;

        if (!hasBaseFloor)
        {
            baseFloorY = hitY;
            hasBaseFloor = true;
            return PlaneCheckResult.Ok;
        }

        float diff = Mathf.Abs(hitY - baseFloorY);

        if (diff > allowedHeightDifference)
            return PlaneCheckResult.DifferentHeight;

        return PlaneCheckResult.Ok;
    }

    public void ResetBaseFloor()
    {
        hasBaseFloor = false;
    }
}