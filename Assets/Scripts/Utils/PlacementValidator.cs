using UnityEngine;

public class PlacementValidator : MonoBehaviour
{
    [SerializeField] private MessageUIManager messageUIManager;

    public bool CanPlace(GameObject prefab, Vector3 position, Quaternion rotation, FurnitureObject ignore = null)
    {
        BoxCollider box = prefab.GetComponentInChildren<BoxCollider>();

        if (box == null)
        {
            messageUIManager.ShowMessage("BoxCollider ‚ª‚ ‚è‚Ü‚¹‚ñ");
            return true;
        }

        Vector3 center = position + rotation * box.center;
        Vector3 halfExtents = Vector3.Scale(box.size, prefab.transform.lossyScale) * 0.5f;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);

        foreach (Collider hit in hits)
        {
            FurnitureObject furniture = hit.GetComponentInParent<FurnitureObject>();

            if (furniture != null && furniture != ignore)
            {
                return false;
            }
        }

        return true;
    }
}