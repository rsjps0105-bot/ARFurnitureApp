using UnityEngine;

public class Furniture : MonoBehaviour
{
    [SerializeField] private Color normalOutlineColor = new Color32(0, 217, 255, 255);
    [SerializeField] private Color errorOutlineColor = Color.red;

    private Outline[] outlines;

    private void Awake()
    {
        outlines = GetComponentsInChildren<Outline>(true);

        SetOutline(false, normalOutlineColor);
    }

    public void Select()
    {
        SetOutline(true, normalOutlineColor);
    }

    public void Deselect()
    {
        SetOutline(false, normalOutlineColor);
    }

    public void SetValidOutline()
    {
        SetOutline(true, normalOutlineColor);
    }

    public void SetErrorOutline()
    {
        SetOutline(true, errorOutlineColor);
    }

    private void SetOutline(bool enabled, Color color)
    {
        foreach (Outline outline in outlines)
        {
            if (outline == null) continue;

            outline.enabled = enabled;
            outline.OutlineColor = color;
        }
    }
}