using UnityEngine;
using UnityEngine.UI;

public class BossWarrior : Enemy
{
    private Animator animator;
    [Header("UI Thanh Máu")]
    public Slider healthBar;
    
    [Header("Cấu hình Hoạt ảnh Boss")]
    public float attackRangeThreshold;
    private bool isAttacking = false;
    

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();

        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health;
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }

    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        if (healthBar != null)
            {
            healthBar.value = health;
            }
    }

    protected override void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= attackRangeThreshold)
        {
            rb.linearVelocity = Vector2.zero; // Đứng yên tại chỗ

            if (!isAttacking && animator != null)
            {
                animator.SetBool("isWalking", false); // Chuyển về Idle đứng thở
            }

            if (Time.time >= nextAttackTime && !isAttacking)
            {
                TriggerRandomAttack();
            }
        }
        // 🏃‍♂️ TRƯỜNG HỢP 2: Ở xa và không bận chém -> Chạy bộ đuổi theo
        else if (!isAttacking)
        {
            base.FixedUpdate(); // Thuật toán bầy đàn của Enemy.cs
            
            if (animator != null) animator.SetBool("isWalking", true);
        }
    }

    void TriggerRandomAttack()
    {
        if (animator != null)
        {
            isAttacking = true;
            int randomAttack = Random.Range(1, 3); // Random 1 hoặc 2

            if (randomAttack == 1)
            {
                animator.SetTrigger("Attack1");
                Debug.Log("Attack1");
            }
            else
            {
                animator.SetTrigger("Attack2");
                Debug.Log("Attack2");
            }

            nextAttackTime = Time.time + attackRate; 
        }
    }

    protected override void Die()
    {
        base.Die();
    }

    public void StartGuardState()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetTrigger("Guard");
        }
    }
    
    public void FinishedAttackInsideAnimation()
    {
        isAttacking = false;
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        // Vô hiệu hóa cắn càn
    }
}