using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class FloorTapDetector : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private PlaneManager planeManager;
    [SerializeField] private MessageUIManager messageUIManager;

    private static readonly List<ARRaycastHit> hits = new();

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        if (planeManager != null && !planeManager.IsFloorConfirmed)
        {
            messageUIManager.ShowMessage("床を映してタップしてください");
        }
    }

    private void Update()
    {
        if (planeManager == null || planeManager.IsFloorConfirmed)
            return;

        if (Touch.activeTouches.Count == 0)
            return;

        Touch touch = Touch.activeTouches[0];

        if (UIInputBlocker.IsPointerOverUI(touch))
            return;

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
            return;

        TryConfirmFloor(touch.screenPosition);
    }

    private void TryConfirmFloor(Vector2 screenPosition)
    {
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in hits)
            {
                ARPlane plane = hit.trackable as ARPlane;

                PlaneCheckResult result =
                    planeManager.ConfirmFloor(
                        plane,
                        hit.pose,
                        screenPosition
                    );

                if (result == PlaneCheckResult.Ok)
                {
                    return;
                }
            }

            messageUIManager.ShowMessage("床として認識できません");
        }
        else
        {
            messageUIManager.ShowMessage("床に当たっていません");
        }
    }
}