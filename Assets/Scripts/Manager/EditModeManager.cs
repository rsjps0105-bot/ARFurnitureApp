using UnityEngine;
using TMPro;

public class EditModeManager : MonoBehaviour
{
    public enum EditMode
    {
        None,
        Add,
        Move,
        Rotate,
        Scale
    }

    public EditMode CurrentMode { get; private set; } = EditMode.None;

    [SerializeField] private SelectionManager selectionManager;

    [SerializeField] private TextMeshProUGUI modeText;

    private void Start()
    {
        SetMode(EditMode.None);
    }

    public void SetAddMode()
    {
        SetMode(EditMode.Add);
    }

    public void SetMoveMode()
    {
        SetMode(EditMode.Move);
    }

    public void SetRotateMode()
    {
        SetMode(EditMode.Rotate);
    }

    public void SetScaleMode()
    {
        SetMode(EditMode.Scale);
    }

    public void SetNoneMode()
    {
        SetMode(EditMode.None);
    }

    private void SetMode(EditMode mode)
    {
        CurrentMode = mode;

        if (modeText != null)
        {
            modeText.text = "Mode : " + mode.ToString();
        }

        Debug.Log("Mode : " + mode);
    }

    public void DeleteSelected()
    {
        Furniture selected = selectionManager.SelectedFurniture;

        if (selected == null)
        {
            modeText.text = "No object selected";
            return;
        }

        Destroy(selected.gameObject);

        selectionManager.ClearSelection();

        modeText.text = "Deleted";
    }
}