using UnityEngine;
using UnityEngine.UI;

public class ModeButtonUI : MonoBehaviour
{
    [SerializeField] private Image moveButtonImage;
    [SerializeField] private Image rotateButtonImage;
    [SerializeField] private Image scaleButtonImage;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;

    public void SelectMove()
    {
        ResetColors();
        moveButtonImage.color = selectedColor;
    }

    public void SelectRotate()
    {
        ResetColors();
        rotateButtonImage.color = selectedColor;
    }

    public void SelectScale()
    {
        ResetColors();
        scaleButtonImage.color = selectedColor;
    }

    public void Clear()
    {
        ResetColors();
    }

    private void ResetColors()
    {
        moveButtonImage.color = normalColor;
        rotateButtonImage.color = normalColor;
        scaleButtonImage.color = normalColor;
    }
}