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
    [SerializeField] private UndoManager undoManager;
    [SerializeField] private Transform furnitureRoot;

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
        if (Touch.activeTouches.Count == 0) return;

        Touch touch = Touch.activeTouches[0];

        if (UIInputBlocker.IsPointerOverUI(touch))
            return;

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
            return;

        if (editModeManager.CurrentMode != EditModeManager.EditMode.Add)
            return;

        if (planeManager != null && !planeManager.IsFloorConfirmed)
        {
            messageUIManager.ShowMessage("先に床を検出してください");
            return;
        }

        if (selectionManager != null && selectionManager.DidClearSelectionThisFrame)
        {
            messageUIManager.ShowMessage("選択解除したため配置しない");
            return;
        }

        if (selectionManager != null && selectionManager.HasSelection)
        {
            messageUIManager.ShowMessage("選択中のため配置しない");
            return;
        }

        if (IsTouchingFurniture(touch.screenPosition))
        {
            messageUIManager.ShowMessage("家具を選択中");
            return;
        }

        TryPlaceObject(touch.screenPosition);
    }

    public void SetPlacePrefab(GameObject prefab)
    {
        placePrefab = prefab;

        editModeManager.SetMode(EditModeManager.EditMode.Add);

        if (planeManager != null && !planeManager.IsFloorConfirmed)
        {
            messageUIManager.ShowMessage("床を検出してから配置できます");
        }
        else
        {
            messageUIManager.ShowMessage("配置する家具を選択しました");
        }
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
        if (placePrefab == null)
        {
            messageUIManager.ShowMessage("配置する家具が選択されていません");
            return;
        }

        if (!raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            messageUIManager.ShowMessage("床に当たっていません");
            return;
        }

        foreach (ARRaycastHit hit in hits)
        {
            ARPlane plane = hit.trackable as ARPlane;

            PlaneCheckResult result = planeManager.CheckPlane(plane, hit.pose);

            if (result != PlaneCheckResult.Ok)
                continue;

            Vector3 placePosition = hit.pose.position;
            Quaternion placeRotation = Quaternion.identity;

            if (!placementValidator.CanPlace(placePrefab, placePosition, placeRotation))
            {
                messageUIManager.ShowMessage("ここには配置できません");
                return;
            }

            GameObject placedObject =
                Instantiate(
                    placePrefab,
                    placePosition,
                    placeRotation,
                    furnitureRoot
                );
            messageUIManager.ShowMessage("家具を配置しました");
            return;
        }

        messageUIManager.ShowMessage("置ける床が見つかりません");
    }
}