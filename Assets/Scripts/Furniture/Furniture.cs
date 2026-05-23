using UnityEngine;

public class Furniture : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;

    private Color defaultColor;
    private Color selectedColor = Color.green;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        defaultColor = targetRenderer.material.color;
    }

    public void Select()
    {
        targetRenderer.material.color = selectedColor;
    }

    public void Deselect()
    {
        targetRenderer.material.color = defaultColor;
    }
}