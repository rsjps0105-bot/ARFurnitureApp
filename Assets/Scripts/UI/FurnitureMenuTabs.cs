using UnityEngine;

public class FurnitureMenuTabs : MonoBehaviour
{
    [SerializeField] private GameObject chairScrollView;
    [SerializeField] private GameObject tableScrollView;
    [SerializeField] private GameObject otherScrollView;

    private void Start()
    {
        ShowChair();
    }

    public void ShowChair()
    {
        chairScrollView.SetActive(true);
        tableScrollView.SetActive(false);
        otherScrollView.SetActive(false);
    }

    public void ShowTable()
    {
        chairScrollView.SetActive(false);
        tableScrollView.SetActive(true);
        otherScrollView.SetActive(false);
    }

    public void ShowOther()
    {
        chairScrollView.SetActive(false);
        tableScrollView.SetActive(false);
        otherScrollView.SetActive(true);
    }
}