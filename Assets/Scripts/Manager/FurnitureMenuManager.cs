using UnityEngine;

public class FurnitureMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject furnitureMenuPanel;

    public void OpenMenu()
    {
        furnitureMenuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        furnitureMenuPanel.SetActive(false);
    }
}