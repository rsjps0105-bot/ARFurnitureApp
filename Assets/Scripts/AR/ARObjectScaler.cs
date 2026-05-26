using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARObjectScaler : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private EditModeManager editModeManager;
    [SerializeField] private PlacementValidator placementValidator;
    [SerializeField] private MessageUIManager messageUIManager;
    [SerializeField] private UndoManager undoManager;

    [SerializeField] private float scaleSpeed = 0.002f;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 1.0f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;

    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    private Vector3 lastValidScale;

    private float previousDistance;

    private bool isScaling = false;
    private bool currentScaleValid = true;

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
        if (editModeManager.CurrentMode != EditModeManager.EditMode.Scale) return;
        if (selectionManager.SelectedFurniture == null) return;
        if (Touch.activeTouches.Count < 2) return;

        Touch touch1 = Touch.activeTouches[0];
        Touch touch2 = Touch.activeTouches[1];

        float currentDistance = Vector2.Distance(
            touch1.screenPosition,
            touch2.screenPosition
        );

        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
        {
            StartScale(currentDistance);
        }
        else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            Scale(currentDistance);
        }
        else if (touch1.phase == TouchPhase.Ended ||
                 touch2.phase == TouchPhase.Ended ||
                 touch1.phase == TouchPhase.Canceled ||
                 touch2.phase == TouchPhase.Canceled)
        {
            EndScale();
        }
    }

    private void StartScale(float currentDistance)
    {
        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        startPosition = selected.transform.position;
        startRotation = selected.transform.rotation;
        startScale = selected.transform.localScale;

        lastValidPosition = selected.transform.position;
        lastValidRotation = selected.transform.rotation;
        lastValidScale = selected.transform.localScale;

        previousDistance = currentDistance;

        isScaling = true;
        currentScaleValid = true;

        messageUIManager.ShowMessage("ägëÂèkè¨íÜ");
    }

    private void Scale(float currentDistance)
    {
        if (!isScaling) return;

        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        float delta = currentDistance - previousDistance;

        Vector3 currentScale = selected.transform.localScale;
        Vector3 newScale = currentScale + Vector3.one * delta * scaleSpeed;

        float clamped = Mathf.Clamp(newScale.x, minScale, maxScale);
        selected.transform.localScale = Vector3.one * clamped;

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
            lastValidScale = selected.transform.localScale;
            currentScaleValid = true;

            selectionManager.SelectedFurniture.SetValidOutline();

            messageUIManager.ShowMessage("ägëÂèkè¨Ç≈Ç´Ç‹Ç∑");
        }
        else
        {
            currentScaleValid = false;

            selectionManager.SelectedFurniture.SetErrorOutline();
        }

        previousDistance = currentDistance;
    }

    private void EndScale()
    {
        if (!isScaling) return;

        GameObject selected = selectionManager.SelectedFurniture.gameObject;

        if (!currentScaleValid)
        {
            selected.transform.position = lastValidPosition;
            selected.transform.rotation = lastValidRotation;
            selected.transform.localScale = lastValidScale;

            messageUIManager.ShowMessage("íºëOÇÃëÂÇ´Ç≥Ç…ñﬂÇµÇ‹ÇµÇΩ");
        }
        else
        {
            Vector3 undoPosition = startPosition;
            Quaternion undoRotation = startRotation;
            Vector3 undoScale = startScale;

            undoManager.RegisterUndo(() =>
            {
                if (selected != null)
                {
                    selected.transform.position = undoPosition;
                    selected.transform.rotation = undoRotation;
                    selected.transform.localScale = undoScale;
                }
            });

            messageUIManager.ShowMessage("ëÂÇ´Ç≥ÇïœçXÇµÇ‹ÇµÇΩ");
        }

        selectionManager.SelectedFurniture.SetValidOutline();
        isScaling = false;
    }
}