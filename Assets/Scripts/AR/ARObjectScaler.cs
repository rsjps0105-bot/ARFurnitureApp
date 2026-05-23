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

    [SerializeField] private float scaleSpeed = 0.002f;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 1.0f;

    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    private Vector3 lastValidScale;

    private float previousDistance;

    private bool isScaling = false;
    private bool currentScaleValid = true;

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

        lastValidPosition = selected.transform.position;
        lastValidRotation = selected.transform.rotation;
        lastValidScale = selected.transform.localScale;

        previousDistance = currentDistance;

        isScaling = true;
        currentScaleValid = true;

        messageUIManager.ShowMessage("Šg‘åk¬’†");
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

            messageUIManager.ShowMessage("Šg‘åk¬‚Å‚«‚Ü‚·");
        }
        else
        {
            currentScaleValid = false;
            messageUIManager.ShowMessage("‘¼‚Ì‰Æ‹ï‚Æd‚È‚Á‚Ä‚¢‚Ü‚·");
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

            messageUIManager.ShowMessage("’¼‘O‚Ì‘å‚«‚³‚É–ß‚µ‚Ü‚µ‚½");
        }
        else
        {
            messageUIManager.ShowMessage("‘å‚«‚³‚ð•ÏX‚µ‚Ü‚µ‚½");
        }

        isScaling = false;
    }
}