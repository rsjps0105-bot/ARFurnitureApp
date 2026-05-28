using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public Furniture SelectedFurniture { get; private set; }

    public bool HasSelection => SelectedFurniture != null;

    public bool DidClearSelectionThisFrame { get; private set; }

    public void Select(Furniture furniture)
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