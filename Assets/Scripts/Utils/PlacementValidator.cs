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
        if (useDepthValidation && depthPlacementValidator != null)
        {
            if (!depthPlacementValidator.CanPlaceAtWorldPosition(position))
            {
                messageUIManager.ShowMessage("ï«Ç‚ï®ëÃÇ…ñÑÇ‡ÇÍÇÈà íuÇ…ÇÕíuÇØÇ‹ÇπÇÒ");
                return false;
            }
        }

        BoxCollider box = prefab.GetComponentInChildren<BoxCollider>();

        if (box == null)
        {
            messageUIManager.ShowMessage("BoxCollider Ç™Ç†ÇËÇ‹ÇπÇÒ");
            return true;
        }

        Vector3 center = position + rotation * box.center;
        Vector3 halfExtents =
            Vector3.Scale(box.size, prefab.transform.lossyScale) * 0.5f;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);

        foreach (Collider hit in hits)
        {
            Furniture furniture = hit.GetComponentInParent<Furniture>();

            if (furniture != null && furniture != ignore)
            {
                messageUIManager.ShowMessage("ëºÇÃâ∆ãÔÇ∆èdÇ»Ç¡ÇƒÇ¢Ç‹Ç∑");
                return false;
            }
        }

        return true;
    }
}