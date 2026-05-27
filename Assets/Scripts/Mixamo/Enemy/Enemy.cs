using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyMotor))]
[RequireComponent(typeof(EnemyVision))]
public class Enemy : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private MessageUIManager messageUIManager;

    // Components
    public Animator Anim { get; private set; }
    public EnemyMotor Motor { get; private set; }
    public EnemyVision Vision { get; private set; }

    // Animator Params
    public readonly int SpeedParam = Animator.StringToHash("Speed");
    public readonly int AttackParam = Animator.StringToHash("Attack");

    // Attack Settings
    [Header("Attack")]
    public float attackCooldown = 1.0f;
    [HideInInspector] public float nextAttackTime;
    public float attackDamage = 1f;
    public float attackRadius = 0.7f;
    public float attackDistance = 1.2f;
    public LayerMask attackMask;
    public Transform attackOrigin;

    // StateMachine
    public EnemyStateMachine SM { get; private set; }

    public EnemyLocomotionState LocomotionState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }

    void Awake()
    {
        Anim = GetComponent<Animator>();
        Motor = GetComponent<EnemyMotor>();
        Vision = GetComponent<EnemyVision>();

        SM = new EnemyStateMachine();
        LocomotionState = new EnemyLocomotionState(this);
        AttackState = new EnemyAttackState(this);

        if (messageUIManager == null)
            messageUIManager = FindAnyObjectByType<MessageUIManager>();
    }

    void Start()
    {
        // èâä˙State
        SM.ChangeState(LocomotionState);
    }

    void Update()
    {
        Vision.FindNearestFurniture();

        SM.Tick();
    }

    // AnimEvent Ç©ÇÁåƒÇŒÇÍÇÈ
    public void AnimEvent_AttackHit()
    {
        Transform origin = attackOrigin != null ? attackOrigin : transform;

        Vector3 center = origin.position + origin.forward * attackDistance;

        Collider[] hits = Physics.OverlapSphere(
            center,
            attackRadius,
            attackMask,
            QueryTriggerInteraction.Ignore);

        foreach (var col in hits)
        {
            messageUIManager.ShowMessage($"ìñÇΩÇ¡ÇΩCollider: {col.name}");

            var hp = col.GetComponentInParent<Health>();

            if (hp != null)
            {
                messageUIManager.ShowMessage($"Healthî≠å©: {hp.name}");

                hp.TakeDamage(attackDamage);
                Debug.Log($"{name} hit {col.name}");
                break;
            }
        }
    }

    // â¬éãâª
    void OnDrawGizmosSelected()
    {
        Transform origin = attackOrigin != null ? attackOrigin : transform;

        Vector3 center = origin.position + origin.forward * attackDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}