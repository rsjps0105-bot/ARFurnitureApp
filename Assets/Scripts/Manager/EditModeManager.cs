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
        Scale,
        Delete
    }

    public EditMode CurrentMode { get; private set; } = EditMode.None;

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

    public void SetDeleteMode()
    {
        SetMode(EditMode.Delete);
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
}