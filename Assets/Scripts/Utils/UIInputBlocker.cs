using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public static class UIInputBlocker
{
    public static bool IsPointerOverUI(Touch touch)
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject(touch.touchId);
    }
}