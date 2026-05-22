using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int health = 3;
    protected Transform player; // Đổi sang protected
    protected Rigidbody2D rb;    // Đổi sang protected
    protected SpriteRenderer spriteRenderer; // Đổi sang protected
    [Header("Hiệu ứng khi chết")] public GameObject deathEffectPrefab;
    [Header("Sát thương quái")] public float enemyDamage = 10;
    public float attackRate = 0.5f;
    protected float nextAttackTime; // Đổi sang protected để con đọc được

    // Thêm chữ protected virtual vào đầu các hàm này
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (player != null)
        {
            Vector2 targetDirection = (player.position - transform.position).normalized;

            // --- THUẬT TOÁN BẦY ĐÀN (SEPARATION) --- CỦA CHẤN
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
                targetDirection += separationTarget;
                targetDirection = targetDirection.normalized;
            }
            
            rb.linearVelocity = targetDirection * moveSpeed;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = targetDirection.x <= 0;
            }
        }
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    // ĐỔI THÀNH protected virtual ĐỂ BOSS KẾ THỪA
    protected virtual void Die()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    // Hàm cắn càn dành riêng cho MOB THƯỜNG
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.isTrigger)
        {
            if (Time.time >= nextAttackTime)
            {
                PlayerController playerController = collision.GetComponentInParent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(enemyDamage);
                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
    }
}