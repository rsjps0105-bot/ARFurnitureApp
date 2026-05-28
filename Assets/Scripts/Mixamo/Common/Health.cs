using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("HP")]
    public float maxHP = 5f;

    [Header("Death")]
    public float destroyDelay = 1f;

    [Header("UI")]
    [SerializeField] private MessageUIManager messageUIManager;

    public float CurrentHP { get; private set; }
    public bool IsDead => CurrentHP <= 0f;

    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onDied;

    Animator anim;
    Collider[] cols;

    void Awake()
    {
        CurrentHP = maxHP;
        anim = GetComponent<Animator>();
        cols = GetComponentsInChildren<Collider>();

        if (messageUIManager == null)
            messageUIManager = FindAnyObjectByType<MessageUIManager>();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        if (messageUIManager != null)
        {
            messageUIManager.ShowMessage(
                $"家具が {damage} ダメージを受けた！");
        }

        CurrentHP -= damage;
        CurrentHP = Mathf.Max(CurrentHP, 0f);

        if (anim != null)
            anim.SetTrigger("Hit");

        onDamaged?.Invoke();

        if (IsDead)
            Die();
    }

    void Die()
    {
        if (anim != null)
            anim.SetTrigger("Die");

        if (messageUIManager != null)
        {
            messageUIManager.ShowMessage(
                $"{name} が破壊された");
        }

        onDied?.Invoke();

        DisableAfterDeath();

        StartCoroutine(DestroyAfterDelay());
    }

    void DisableAfterDeath()
    {
        foreach (var col in cols)
            col.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        var enemy = GetComponent<Enemy>();
        if (enemy != null)
            enemy.enabled = false;

        var nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null)
            nav.enabled = false;
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}