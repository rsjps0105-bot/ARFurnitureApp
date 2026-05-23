using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARObjectMover : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private EditModeManager editModeManager;
    [SerializeField] private PlacementValidator placementValidator;
    [SerializeField] private MessageUIManager messageUIManager;

    private static readonly List<ARRaycastHit> hits = new();

    private Vector3 grabOffset;

    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;

    private bool isDragging = false;
    private bool currentPositionValid = true;

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
        if (editModeManager.CurrentMode != EditModeManager.EditMode.Move) return;
        if (selectionManager.SelectedFurniture == null) return;
        if (Touch.activeTouches.Count == 0) return;

        Touch touch = Touch.activeTouches[0];

        if (touch.phase == TouchPhase.Began)
        {
            StartMove(touch.screenPosition);
        }
        else if (touch.phase == TouchPhase.Moved || // タップ移動中
                 touch.phase == TouchPhase.Stationary) // タップしているが指が動いていない場合も含む
        {
            Move(touch.screenPosition);
        }
        else if (touch.phase == TouchPhase.Ended ||
                 touch.phase == TouchPhase.Canceled)
        {
            EndMove();
        }
    }

    private void StartMove(Vector2 screenPosition)
    {
        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        lastValidPosition = selected.transform.position;
        lastValidRotation = selected.transform.rotation;

        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // タップした床位置から見た家具中心までのズレを保存
            grabOffset = selected.transform.position - hitPose.position;
        }
        else
        {
            grabOffset = Vector3.zero;
        }

        isDragging = true;
        currentPositionValid = true;

        messageUIManager.ShowMessage("移動中");
    }

    private void Move(Vector2 screenPosition)
    {
        if (!isDragging) return;

        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        // タップ位置からAR平面へのRaycastを行う
        if (!raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            return;
        }

        // 最も近い床上の位置を取得
        Pose hitPose = hits[0].pose;

        // 指の位置に対応する床の場所へ家具を移動
        selected.transform.position = hitPose.position + grabOffset;

        bool canPlace = placementValidator.CanPlace(
            selected,
            selected.transform.position,
            selected.transform.rotation,
            selectionManager.SelectedFurniture
        );

        if (canPlace)
        {
            lastValidPosition = selected.transform.position;
            lastValidRotation = selected.transform.rotation;
            currentPositionValid = true;

            messageUIManager.ShowMessage("移動できます");
        }
        else
        {
            currentPositionValid = false;

            messageUIManager.ShowMessage("他の家具と重なっています");
        }
    }

    private void EndMove()
    {
        if (!isDragging) return;

        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        if (!currentPositionValid)
        {
            selected.transform.position = lastValidPosition;
            selected.transform.rotation = lastValidRotation;

            messageUIManager.ShowMessage("直前の位置に戻しました");
        }
        else
        {
            messageUIManager.ShowMessage("移動しました");
        }

        isDragging = false;
    }
}