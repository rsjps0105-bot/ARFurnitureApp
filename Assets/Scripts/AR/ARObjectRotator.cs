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
    [SerializeField] private UndoManager undoManager;

    [SerializeField] private float rotationSpeed = 0.2f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;

    private bool isRotating = false;
    private bool currentRotationValid = true;

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

        startPosition = selected.transform.position;
        startRotation = selected.transform.rotation;

        lastValidPosition = selected.transform.position;
        lastValidRotation = selected.transform.rotation;

        isRotating = true;
        currentRotationValid = true;

        messageUIManager.ShowMessage("‰ñ“]’†");
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

            messageUIManager.ShowMessage("‰ñ“]‚Å‚«‚Ü‚·");
        }
        else
        {
            currentRotationValid = false;
            messageUIManager.ShowMessage("‘¼‚Ì‰Æ‹ï‚Æd‚È‚Á‚Ä‚¢‚Ü‚·");
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

            messageUIManager.ShowMessage("’¼‘O‚ÌŠp“x‚É–ß‚µ‚Ü‚µ‚½");
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

            messageUIManager.ShowMessage("‰ñ“]‚µ‚Ü‚µ‚½");
        }

        isRotating = false;
    }
}