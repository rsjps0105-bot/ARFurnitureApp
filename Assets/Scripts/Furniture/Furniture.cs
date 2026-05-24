using UnityEngine;

public class Furniture : MonoBehaviour
{
    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Select()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void Deselect()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}