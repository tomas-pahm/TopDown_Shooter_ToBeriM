using UnityEngine;

public class BossMonk : Enemy
{
    private Animator animator;

    [Header("Cấu hình Hồi Máu của Monk")]
    public float healRate = 3f;
    public int healAmount = 5;
    public float healRange = 10f;
    public LayerMask enemyLayer;
    public GameObject healEffectPrefab;
    private float nextHealTime;

    [Header("AI Bám Đuôi & Trốn Chạy")]
    public float followDistance = 3f;       
    public float panicRange = 5f;         
    public float speedBuffMultiplier = 1.8f; 

    private Transform warriorTarget;        
    protected override void Start()
    {
        base.Start(); 
        animator = GetComponentInChildren<Animator>();
        nextHealTime = Time.time + healRate;
        
        BossWarrior warriorScript = FindFirstObjectByType<BossWarrior>();
        if (warriorScript != null)
        {
            warriorTarget = warriorScript.transform;
        }
    }

    protected override void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float currentMoveSpeed = moveSpeed; 
        
        if (distanceToPlayer < panicRange)
        {
            currentMoveSpeed = moveSpeed * speedBuffMultiplier; 
            
            Vector2 escapeDirection;
            
            if (warriorTarget != null)
            {
                escapeDirection = (warriorTarget.position - transform.position).normalized;
                Debug.Log("😱 Cứu em đại ca ơi! Monk đang chạy về phía Warrior!");
            }
            else
            {
                escapeDirection = (transform.position - player.position).normalized;
            }
            
            escapeDirection = ApplySeparation(escapeDirection);

            rb.linearVelocity = escapeDirection * currentMoveSpeed;
            if (animator != null) animator.SetBool("isWalking", true);
            if (spriteRenderer != null) spriteRenderer.flipX = escapeDirection.x <= 0;
        }
        
        else if (warriorTarget != null)
        {
            float distanceToWarrior = Vector2.Distance(transform.position, warriorTarget.position);
            
            if (distanceToWarrior > followDistance)
            {
                Vector2 followDirection = (warriorTarget.position - transform.position).normalized;
                followDirection = ApplySeparation(followDirection);

                rb.linearVelocity = followDirection * currentMoveSpeed;
                if (animator != null) animator.SetBool("isWalking", true);
                if (spriteRenderer != null) spriteRenderer.flipX = followDirection.x <= 0;
            }
            else
            {
                StopAndHeal();
            }
        }
        else
        {
            StopAndHeal();
        }
    }

    protected override void Die()
    {
        base.Die();
    }

    private void StopAndHeal()
    {
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetBool("isWalking", false);

        if (Time.time >= nextHealTime)
        {
            HealAllAllies();
        }
    }
    
    private Vector2 ApplySeparation(Vector2 currentDirection)
    {
        Vector2 separationTarget = Vector2.zero;
        int neighborCount = 0;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, 0.6f);
        
        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject != gameObject && neighbor.CompareTag("Enemy"))
            {
                separationTarget += (Vector2)(transform.position - neighbor.transform.position);
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            separationTarget /= neighborCount;
            currentDirection += separationTarget;
            currentDirection = currentDirection.normalized;
        }
        return currentDirection;
    }

    void HealAllAllies()
    {
        if (animator != null)
        {
            animator.SetTrigger("Heal");
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayVFX(AudioManager.Instance.healSound);
            }
        }
        Debug.Log("🧙‍♂️ Monk gồng phép hồi máu!");

        Collider2D[] allies = Physics2D.OverlapCircleAll(transform.position, healRange, enemyLayer);
        foreach (var ally in allies)
        {
            if(!ally.isTrigger) continue;
            Enemy enemyScript = ally.GetComponent<Enemy>();
            if (enemyScript != null && ally.gameObject != gameObject)
            {
                enemyScript.Heal(healAmount);
                Debug.Log($"💚 Đã hồi {healAmount} máu cho {ally.name}.");
                if (healEffectPrefab != null)
                {
                    GameObject healVFX = Instantiate(healEffectPrefab, ally.transform.position, Quaternion.identity);
                    healVFX.transform.SetParent(ally.transform);
                    Destroy(healVFX, 1f);
                }
            }
        }
        nextHealTime = Time.time + healRate;
    }
}