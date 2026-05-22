using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class FurnitureSelector : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private SelectionManager selectionManager;

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
        if (Touch.activeTouches.Count == 0)
            return;

        Touch touch = Touch.activeTouches[0];

        // UIをタップしている場合は選択しない
        if (UIInputBlocker.IsPointerOverUI(touch))
            return;

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
            return;

        TrySelect(touch.screenPosition);
    }

    private void TrySelect(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            FurnitureObject furniture = hit.collider.GetComponentInParent<FurnitureObject>();

            if (furniture != null)
            {
                selectionManager.Select(furniture);
                return;
            }
        }

        selectionManager.ClearSelection();
    }
}