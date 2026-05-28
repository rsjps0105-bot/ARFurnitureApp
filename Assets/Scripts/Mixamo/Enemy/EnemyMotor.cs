using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMotor : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float turnSpeed = 10f;

    CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void MoveTo(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            return;

        // ‰ñ“]
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            turnSpeed * Time.deltaTime);

        // ˆÚ“®
        Vector3 move = dir.normalized * moveSpeed * Time.deltaTime;
        controller.Move(move);
    }

    public void FaceTo(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
    }
}