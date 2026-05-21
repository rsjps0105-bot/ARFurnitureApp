using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public FurnitureObject SelectedFurniture { get; private set; }

    public bool HasSelection => SelectedFurniture != null;

    public bool DidClearSelectionThisFrame { get; private set; }

    public void Select(FurnitureObject furniture)
    {
        DidClearSelectionThisFrame = false;

        if (SelectedFurniture != null)
        {
            SelectedFurniture.Deselect();
        }

        SelectedFurniture = furniture;

        if (SelectedFurniture != null)
        {
            SelectedFurniture.Select();
        }
    }

    public void ClearSelection()
    {
        if (SelectedFurniture != null)
        {
            SelectedFurniture.Deselect();
            SelectedFurniture = null;
            DidClearSelectionThisFrame = true;
        }
    }

    private void LateUpdate()
    {
        DidClearSelectionThisFrame = false;
    }
}