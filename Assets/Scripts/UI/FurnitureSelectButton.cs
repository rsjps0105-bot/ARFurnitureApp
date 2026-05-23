using UnityEngine;
using UnityEngine.UI;

public class FurnitureSelectButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject furniturePrefab;
    [SerializeField] private ARObjectPlacer objectPlacer;
    [SerializeField] private GameObject furnitureMenuPanel;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        button.onClick.AddListener(SelectFurniture);
    }

    private void SelectFurniture()
    {
        objectPlacer.SetPlacePrefab(furniturePrefab);
        furnitureMenuPanel.SetActive(false);
    }
}