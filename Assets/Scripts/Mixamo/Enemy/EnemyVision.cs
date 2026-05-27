using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Vision")]
    public float viewDistance = 8f;
    [Range(0, 360)]
    public float viewAngle = 120f;

    [Header("Attack")]
    public float attackDistance = 1.6f;

    [Header("Mask")]
    public LayerMask obstacleMask;

    [Header("Refs")]
    public Transform eye;

    void Start()
    {
        InvokeRepeating(
            nameof(FindNearestFurniture),
            0f,
            0.5f);
    }

    public void FindNearestFurniture()
    {
        GameObject[] furnitures =
            GameObject.FindGameObjectsWithTag("Furniture");

        float closest = Mathf.Infinity;

        target = null;

        foreach (var furniture in furnitures)
        {
            float dist =
                Vector3.Distance(
                    transform.position,
                    furniture.transform.position);

            if (dist < closest)
            {
                closest = dist;
                target = furniture.transform;
            }
        }
    }

    public bool CanSeeTarget()
    {
        if (target == null) return false;

        Vector3 origin = eye ? eye.position : transform.position + Vector3.up * 1.5f;

        // プレイヤーの胸あたりを見る（足元だと床/段差に邪魔されやすい）
        Vector3 targetPos = target.position + Vector3.up * 1.0f;
        Vector3 dir = targetPos - origin;

        // デバッグ用
        Debug.DrawRay(origin, dir.normalized * viewDistance, Color.red);

        float dist = dir.magnitude;
        if (dist > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > viewAngle * 0.5f) return false;

        // 障害物レイヤーだけを見る（ObstacleMaskに壁だけ入れる）
        if (Physics.Raycast(origin, dir.normalized, dist, obstacleMask, QueryTriggerInteraction.Ignore))
            return false;

        return true;
    }

    public bool InAttackRange()
    {
        if (target == null) return false;

        Collider col = target.GetComponent<Collider>();

        if (col == null)
            return false;

        Vector3 closest =
            col.ClosestPoint(transform.position);

        Vector3 a = transform.position;
        Vector3 b = closest;

        a.y = 0;
        b.y = 0;

        return Vector3.Distance(a, b) <= attackDistance;
    }
}