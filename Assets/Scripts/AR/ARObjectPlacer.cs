using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ARObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject placePrefab;
    [SerializeField] private PlaneManager planeManager;

    private ARRaycastManager raycastManager;
    private static readonly List<ARRaycastHit> hits = new();

    private string message = "待機中";

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        if (Touch.activeTouches.Count == 0) return;

        var touch = Touch.activeTouches[0];

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began) return;

        message = "タップされた";

        if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach (var hit in hits)
            {
                ARPlane plane = hit.trackable as ARPlane;

                PlaneCheckResult result = planeManager.CheckPlane(plane, hit.pose);

                if (result == PlaneCheckResult.Ok)
                {
                    message = "有効な床に配置";
                    Instantiate(placePrefab, hit.pose.position, Quaternion.identity);
                    return;
                }

                switch (result)
                {
                    case PlaneCheckResult.TooSmall:
                        message = "床が小さい";
                        break;

                    case PlaneCheckResult.DifferentHeight:
                        message = "高さが違う";
                        break;

                    case PlaneCheckResult.NotHorizontal:
                        message = "床ではない";
                        break;

                    default:
                        message = "配置できない";
                        break;
                }
            }

            message = "置ける床が見つからない";
        }
        else
        {
            message = "床に当たってない";
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = Color.red;

        GUI.Label(new Rect(50, 50, 1000, 100), message, style);
    }
}