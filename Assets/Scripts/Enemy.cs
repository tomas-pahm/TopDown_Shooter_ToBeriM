using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int health = 3;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    [Header("Hiệu ứng khi chết")] public GameObject deathEffectPrefab;
    [Header("Sát thương quái")] public float enemyDamage = 10;
    public float attackRate = 0.5f;
    private float nextAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Tìm Player theo Tag an toàn
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            Vector2 targetDirection = (player.position - transform.position).normalized;

            // --- THUẬT TOÁN BẦY ĐÀN (SEPARATION) ---
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

            // lật mặt Sprite quái
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = targetDirection.x <= 0;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    void Die()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

   
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.isTrigger)
            {
                if (Time.time >= nextAttackTime)
                {
                    PlayerController playerController = collision.GetComponent<PlayerController>();
                    if (playerController == null)
                    {
                        playerController = collision.GetComponentInParent<PlayerController>();
                    }

                    if (playerController != null)
                    {
                        playerController.TakeDamage(enemyDamage);
                        
                        nextAttackTime = Time.time + attackRate;
                    }
                }
            }
        }
    }
}