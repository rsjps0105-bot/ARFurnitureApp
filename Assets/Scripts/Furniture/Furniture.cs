using UnityEngine;

public class Furniture : MonoBehaviour
{
    private Outline outline;

    [SerializeField] private Color normalOutlineColor = new Color32(0, 217, 255, 255);
    [SerializeField] private Color errorOutlineColor = Color.red;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
        {
            outline.enabled = false;
            outline.OutlineColor = normalOutlineColor;
        }
    }

    public void Select()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = normalOutlineColor;
        }
    }

    public void Deselect()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void SetValidOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = normalOutlineColor;
        }
    }

    public void SetErrorOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = errorOutlineColor;
        }
    }
}