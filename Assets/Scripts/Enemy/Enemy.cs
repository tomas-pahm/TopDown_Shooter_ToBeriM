using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float moveSpeed = 3f;
    public int health = 3;
    protected Transform player; // Đổi sang protected
    protected Rigidbody2D rb;    // Đổi sang protected
    protected SpriteRenderer spriteRenderer;
    [Header("Cấu hình XP")] public int xpDropAmount;// Đổi sang protected
    [Header("Hiệu ứng khi chết")] 
    public GameObject deathEffectPrefab;
    public GameObject xpOrbPrefab;
    [Header("Sát thương quái")] public float enemyDamage = 10;
    public float attackRate = 0.5f;
    
    [Header("Homing Missile")]
    public bool isItBullet = false;
    public float bulletLifeTime = 5f;
    
    protected float nextAttackTime;
    protected int maxHealth;
    protected bool isDead = false;

    // Thêm chữ protected virtual vào đầu các hàm này
    protected virtual void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        maxHealth = health;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (isItBullet)
        {
            Destroy(gameObject, bulletLifeTime);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isDead || player == null) return;
        
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
                spriteRenderer.flipX = targetDirection.x <= 0;
            }
        
    }

    public virtual void Heal(int amount)
    {
        if (isDead) return;
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVFX(AudioManager.Instance.enemyHitSound);
        }
        if (health <= 0) Die();
    }

    // ĐỔI THÀNH protected virtual ĐỂ BOSS KẾ THỪA
    protected virtual void Die()
    {
        isDead = true;
        
        if (rb != null) rb.linearVelocity = Vector2.zero;
        
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (xpOrbPrefab != null)
        {
            for (int i = 0; i < xpDropAmount; i++)
            {
                Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                Instantiate(xpOrbPrefab, (Vector2)transform.position + randomDirection, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }

    // Hàm cắn càn dành riêng cho MOB THƯỜNG
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;
        
        if (collision.CompareTag("Player") && collision.isTrigger)
        {
            if (Time.time >= nextAttackTime)
            {
                PlayerController playerController = collision.GetComponentInParent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(enemyDamage);
                    nextAttackTime = Time.time + attackRate;
                    if(isItBullet){Destroy(gameObject);}
                }
            }
        }
    }
}