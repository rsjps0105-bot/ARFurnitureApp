using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARObjectMover : MonoBehaviour
{
    [SerializeField] private PlaneManager planeManager;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private EditModeManager editModeManager;
    [SerializeField] private PlacementValidator placementValidator;
    [SerializeField] private MessageUIManager messageUIManager;
    [SerializeField] private UndoManager undoManager;

    private static readonly List<ARRaycastHit> hits = new();

    private Vector3 grabOffset;

    private Vector3 startPosition;
    private Quaternion startRotation;

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
        else if (touch.phase == TouchPhase.Moved ||
                 touch.phase == TouchPhase.Stationary)
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

        startPosition = selected.transform.position;
        startRotation = selected.transform.rotation;

        lastValidPosition = selected.transform.position;
        lastValidRotation = selected.transform.rotation;

        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            grabOffset = selected.transform.position - hitPose.position;
        }
        else
        {
            grabOffset = Vector3.zero;
        }

        isDragging = true;
        currentPositionValid = true;

        messageUIManager.ShowMessage("ˆÚ“®’†");
    }

    private void Move(Vector2 screenPosition)
    {
        if (!isDragging) return;

        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        if (!raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            currentPositionValid = false;
            selectionManager.SelectedFurniture.SetErrorOutline();
            return;
        }

        ARRaycastHit hit = hits[0];
        ARPlane plane = hit.trackable as ARPlane;

        PlaneCheckResult result = planeManager.CheckPlane(plane, hit.pose);

        if (result != PlaneCheckResult.Ok)
        {
            currentPositionValid = false;
            selectionManager.SelectedFurniture.SetErrorOutline();
            return;
        }

        selected.transform.position = hit.pose.position + grabOffset;

        Vector3 pos = selected.transform.position;
        pos.y = planeManager.PlacementFloorY;
        selected.transform.position = pos;

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

            selectionManager.SelectedFurniture.SetValidOutline();

            messageUIManager.ShowMessage("ˆÚ“®‚Å‚«‚Ü‚·");
        }
        else
        {
            currentPositionValid = false;

            selectionManager.SelectedFurniture.SetErrorOutline();
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

            messageUIManager.ShowMessage("’¼‘O‚ÌˆÊ’u‚É–ß‚µ‚Ü‚µ‚½");
        }
        else
        {
            Vector3 undoPosition = startPosition;
            Quaternion undoRotation = startRotation;

            undoManager.RegisterUndo(() =>
            {
                if (selected != null)
                {
                    selected.transform.position = undoPosition;
                    selected.transform.rotation = undoRotation;
                }
            });

            messageUIManager.ShowMessage("ˆÚ“®‚µ‚Ü‚µ‚½");
        }

        selectionManager.SelectedFurniture.SetValidOutline();
        isDragging = false;
    }
}