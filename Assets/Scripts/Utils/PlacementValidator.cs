using UnityEngine;

public class PlacementValidator : MonoBehaviour
{
    [SerializeField] private MessageUIManager messageUIManager;
    [SerializeField] private DepthPlacementValidator depthPlacementValidator;
    [SerializeField] private bool useDepthValidation = true;

    public bool CanPlace(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        Furniture ignore = null)
    {
        // 深度検査を行う場合、まずはそれをチェックする
        if (useDepthValidation && depthPlacementValidator != null)
        {
            if (!depthPlacementValidator.CanPlaceAtWorldPosition(position))
            {
                messageUIManager.ShowMessage("壁や物体に埋もれる位置には置けません");
                return false;
            }
        }

        BoxCollider box = prefab.GetComponentInChildren<BoxCollider>();

        if (box == null)
        {
            messageUIManager.ShowMessage("BoxCollider がありません");
            return true;
        }

        Vector3 scaledCenter =
            Vector3.Scale(box.center, prefab.transform.lossyScale);

        Vector3 center =
            position + rotation * scaledCenter;

        Vector3 halfExtents =
            Vector3.Scale(box.size, prefab.transform.lossyScale) * 0.5f;

        // 指定したコライダー範囲に家具が重なっていないかをチェック
        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);

        foreach (Collider hit in hits)
        {
            Furniture furniture = hit.GetComponentInParent<Furniture>();

            if (furniture != null && furniture != ignore)
            {
                messageUIManager.ShowMessage("他の家具と重なっています");
                return false;
            }
        }

        return true;
    }
}