using UnityEngine;

public class BatEnemy : Enemy 
{
    [Header("=== CẤU HÌNH DƠI BÁO THỦ ===")]
    public float orbitSpeed = 2f;       // Tốc độ bay vòng tròn quanh Player
    public float dodgeForce = 5f;       // Lực né đạn Vector Pháp Tuyến
    public float radarRadius = 3f;      // Khoảng cách mắt nhìn thấy đạn để né
    public LayerMask playerBulletLayer; // Layer của đạn Player

    [Header("=== CẤU HÌNH ĐẠN ĐUỔI PREDATOR ===")]
    public GameObject homingBulletPrefab; 
    public float shootInterval = 3f;      
    private float shootTimer = 0f;

    private float orbitAngle;
    
    protected override void Start()
    {
        
        base.Start(); 
        
        orbitAngle = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (isDead || player == null) return;
        
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            ShootHomingBullet();
            shootTimer = 0f;
        }
    }
    
    protected override void FixedUpdate()
{
    if (isDead || player == null) return;
    
    float currentAngle = Time.time * orbitSpeed * 50f; 
    
    // Đổi sang Radian để vào Sin Cos
    float rad = currentAngle * Mathf.Deg2Rad;

    Vector3 targetOrbitPos = player.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * 5f;
    
    Vector2 desiredPosition = Vector2.MoveTowards(rb.position, targetOrbitPos, moveSpeed * Time.fixedDeltaTime);
    Vector2 orbitVelocity = (desiredPosition - rb.position) / Time.fixedDeltaTime;

    Vector2 finalVelocity = orbitVelocity;
    
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
        finalVelocity += separationTarget.normalized * (moveSpeed * 0.3f); 
    }
    
    Collider2D[] incomingBullets = Physics2D.OverlapCircleAll(rb.position, radarRadius, playerBulletLayer);
    if (incomingBullets.Length > 0)
    {
        Vector2 dodgeVector = Vector2.zero;
        foreach (Collider2D bullet in incomingBullets)
        {
            if (bullet == null) continue;
            Vector2 awayFromBullet = (rb.position - (Vector2)bullet.transform.position).normalized;
            Vector2 perpendicularDodge = new Vector2(-awayFromBullet.y, awayFromBullet.x);
            dodgeVector += awayFromBullet + perpendicularDodge;
        }
        finalVelocity += dodgeVector.normalized * dodgeForce;
    }
    
    rb.linearVelocity = finalVelocity;

    // Lật mặt Sprite
    if (spriteRenderer != null && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
    {
        spriteRenderer.flipX = rb.linearVelocity.x <= 0;
    }
}

    void ShootHomingBullet()
    {
        if (homingBulletPrefab != null)
        {
            Instantiate(homingBulletPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radarRadius);
    }
}