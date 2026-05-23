using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ARObjectPlacer : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject placePrefab;
    [SerializeField] private PlaneManager planeManager;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private EditModeManager editModeManager;
    [SerializeField] private PlacementValidator placementValidator;
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

    private void Update()
    {
        // タッチがない場合は配置しない
        if (Touch.activeTouches.Count == 0) return;

        Touch touch = Touch.activeTouches[0];

        // UIをタップしている場合は配置しない
        if (UIInputBlocker.IsPointerOverUI(touch))
            return;

        // タップ開始以外は配置しない
        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began) return;

        messageUIManager.ShowMessage("タップされた");

        // 編集モードが配置以外なら、配置しない
        if (editModeManager.CurrentMode != EditModeManager.EditMode.Add)
            return;

        // 選択解除したフレームなら、配置しない
        if (selectionManager != null && selectionManager.DidClearSelectionThisFrame)
        {
            messageUIManager.ShowMessage("選択解除したため配置しない");
            return;
        }

        // 選択中なら、何をタップしても配置しない
        if (selectionManager != null && selectionManager.HasSelection)
        {
            messageUIManager.ShowMessage("選択中のため配置しない");
            return;
        }

        // 家具をタップしているか確認
        if (IsTouchingFurniture(touch.screenPosition))
        {
            messageUIManager.ShowMessage("家具を選択中");
            return;
        }

        // 床に配置
        TryPlaceObject(touch.screenPosition);
    }

    public void SetPlacePrefab(GameObject prefab)
    {
        placePrefab = prefab;

        editModeManager.SetMode(EditModeManager.EditMode.Add);

        messageUIManager.ShowMessage("配置するCubeを選択しました");
    }

    private bool IsTouchingFurniture(Vector2 screenPosition)
    {
        if (arCamera == null)
        {
            messageUIManager.ShowMessage("AR Camera未設定");
            return false;
        }

        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Furniture furniture = hit.collider.GetComponentInParent<Furniture>();

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
                    Vector3 placePosition = hit.pose.position;

                    if (!placementValidator.CanPlace(placePrefab, placePosition, hit.pose.rotation))
                    {
                        messageUIManager.ShowMessage("ここには配置できない");
                        return;
                    }

                    messageUIManager.ShowMessage("有効な床に配置");
                    Instantiate(placePrefab, hit.pose.position, Quaternion.identity);
                    return;
                }

                switch (result)
                {
                    case PlaneCheckResult.TooSmall:
                        messageUIManager.ShowMessage("床が小さい");
                        break;

                    case PlaneCheckResult.DifferentHeight:
                        messageUIManager.ShowMessage("高さが違う");
                        break;

                    case PlaneCheckResult.NotHorizontal:
                        messageUIManager.ShowMessage("床ではない");
                        break;

                    default:
                        messageUIManager.ShowMessage("配置できない");
                        break;
                }
            }

            messageUIManager.ShowMessage("置ける床が見つからない");
        }
        else
        {
            messageUIManager.ShowMessage("床に当たってない");
        }
    }
}