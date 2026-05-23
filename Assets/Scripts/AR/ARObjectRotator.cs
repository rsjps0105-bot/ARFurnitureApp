using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARObjectRotator : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private EditModeManager editModeManager;
    [SerializeField] private PlacementValidator placementValidator;
    [SerializeField] private MessageUIManager messageUIManager;

    [SerializeField] private float rotationSpeed = 0.2f;

    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;

    private bool isRotating = false;
    private bool currentRotationValid = true;

    private void Update()
    {
        if (editModeManager.CurrentMode != EditModeManager.EditMode.Rotate) return;
        if (selectionManager.SelectedFurniture == null) return;
        if (Touch.activeTouches.Count == 0) return;

        Touch touch = Touch.activeTouches[0];

        if (touch.phase == TouchPhase.Began)
        {
            StartRotate();
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Rotate(touch.delta.x);
        }
        else if (touch.phase == TouchPhase.Ended ||
                 touch.phase == TouchPhase.Canceled)
        {
            EndRotate();
        }
    }

    private void StartRotate()
    {
        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        lastValidPosition = selected.transform.position;
        lastValidRotation = selected.transform.rotation;

        isRotating = true;
        currentRotationValid = true;

        messageUIManager.ShowMessage("âÒì]íÜ");
    }

    private void Rotate(float deltaX)
    {
        if (!isRotating) return;

        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        selected.transform.Rotate(
            0,
            -deltaX * rotationSpeed,
            0,
            Space.World
        );

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
            currentRotationValid = true;

            messageUIManager.ShowMessage("âÒì]Ç≈Ç´Ç‹Ç∑");
        }
        else
        {
            currentRotationValid = false;
            messageUIManager.ShowMessage("ëºÇÃâ∆ãÔÇ∆èdÇ»Ç¡ÇƒÇ¢Ç‹Ç∑");
        }
    }

    private void EndRotate()
    {
        if (!isRotating) return;

        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        if (!currentRotationValid)
        {
            selected.transform.position = lastValidPosition;
            selected.transform.rotation = lastValidRotation;

            messageUIManager.ShowMessage("íºëOÇÃäpìxÇ…ñﬂÇµÇ‹ÇµÇΩ");
        }
        else
        {
            messageUIManager.ShowMessage("âÒì]ÇµÇ‹ÇµÇΩ");
        }

        isRotating = false;
    }
}