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
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private Camera arCamera;

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

        Touch touch = Touch.activeTouches[0];

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began) return;

        message = "タップされた";

        if (selectionManager != null && selectionManager.DidClearSelectionThisFrame)
        {
            message = "選択解除したため配置しない";
            return;
        }

        // ① 選択中なら、何をタップしても配置しない
        if (selectionManager != null && selectionManager.HasSelection)
        {
            message = "選択中のため配置しない";
            return;
        }

        // ② 家具をタップしているか確認
        if (IsTouchingFurniture(touch.screenPosition))
        {
            message = "家具を選択中";
            return;
        }

        // ③ 床に配置
        TryPlaceObject(touch.screenPosition);
    }

    private bool IsTouchingFurniture(Vector2 screenPosition)
    {
        if (arCamera == null)
        {
            message = "AR Camera未設定";
            return false;
        }

        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            FurnitureObject furniture = hit.collider.GetComponentInParent<FurnitureObject>();

            if (furniture != null)
            {
                return true;
            }
        }

        return false;
    }

    private void TryPlaceObject(Vector2 screenPosition)
    {
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in hits)
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