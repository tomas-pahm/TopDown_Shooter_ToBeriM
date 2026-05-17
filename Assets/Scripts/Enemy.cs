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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null)
        {
            Vector2 targetDirection = (player.position - transform.position).normalized;

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
                if (targetDirection.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.flipX = true;
                }
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
        if (collision.gameObject.tag == "Player")
        {
            if (Time.time >= nextAttackTime)
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(enemyDamage);
                }

                nextAttackTime = Time.time + attackRate;
            }
        }
    }
}