using UnityEngine;

public class ResetManager : MonoBehaviour
{
    [SerializeField] private PlaneManager planeManager;
    [SerializeField] private Transform furnitureRoot;
    [SerializeField] private MessageUIManager messageUIManager;

    public void ResetAll()
    {
        // 家具削除
        foreach (Transform child in furnitureRoot)
        {
            Destroy(child.gameObject);
        }

        // ARリセット
        if (planeManager != null)
        {
            planeManager.ResetBaseFloor();
        }

        messageUIManager.ShowMessage(
            "ARと配置した家具をリセットしました"
        );
    }
}