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
    private bool isCastingHeal = false; // 🔮 BIẾN BẢO HIỂM: Khóa trạng thái khi đang niệm chú

    protected override void Start()
    {
        base.Start(); 
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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

        // 🔮 ƯU TIÊN TỐI CAO: Nếu đang bận niệm chú hồi máu thì ĐỨNG IM, cấm chạy code di chuyển dưới!
        if (isCastingHeal)
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("isWalking", false);
            return; 
        }

        // ⏰ ĐẾN GIỜ HOÀNG ĐẠO: Cứ đúng lịch là khựng lại niệm chú, bất kể đang đi hay đang đứng!
        if (Time.time >= nextHealTime)
        {
            StartHealCasting();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float currentMoveSpeed = moveSpeed; 

        // 🎯 KỊCH BẢN 1: PLAYER ÁP SÁT -> HOẢNG SỢ & BUFF TỐC CHẠY
        if (distanceToPlayer < panicRange)
        {
            currentMoveSpeed = moveSpeed * speedBuffMultiplier; 
            Vector2 escapeDirection;

            if (warriorTarget != null)
            {
                escapeDirection = (warriorTarget.position - transform.position).normalized;
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
        // 🎯 KỊCH BẢN 2: AN TOÀN -> ĐUỔI THEO BÁM ĐUÔI WARRIOR
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
                // Đang đứng gần đại ca rồi thì đứng im nghỉ ngơi
                rb.linearVelocity = Vector2.zero;
                if (animator != null) animator.SetBool("isWalking", false);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("isWalking", false);
        }
    }

    // Hàm kích hoạt niệm chú
    void StartHealCasting()
    {
        isCastingHeal = true; // Khóa chân di chuyển
        rb.linearVelocity = Vector2.zero; // Ép vận tốc về 0
        
        if (animator != null)
        {
            animator.SetBool("isWalking", false); // Tắt hoạt ảnh đi bộ để tránh bị nuốt chiêu
            animator.SetTrigger("Heal"); // Kích hoạt bốc sao lấp lánh
        }

        Debug.Log("🧙‍♂️ Monk khựng lại gồng phép hồi máu!");
        
        // Thực hiện quét map bơm máu và đẻ hiệu ứng dưới chân đồng bọn luôn
        ExecuteHealAllAllies();

        // Hẹn giờ: Diễn xong hoạt ảnh 1 giây thì mở khóa cho đi tiếp
        Invoke("ResetCastingState", 1f); 
    }

    void ExecuteHealAllAllies()
    {
        Collider2D[] allies = Physics2D.OverlapCircleAll(transform.position, healRange, enemyLayer);
        foreach (var ally in allies)
        {
            Enemy enemyScript = ally.GetComponent<Enemy>();
            if (enemyScript != null && ally.gameObject != gameObject)
            {
                enemyScript.health += healAmount;
                Debug.Log($"💚 Đã hồi {healAmount} máu cho {ally.name}.");

                if (healEffectPrefab != null)
                {
                    GameObject healVFX = Instantiate(healEffectPrefab, ally.transform.position, Quaternion.identity);
                    healVFX.transform.SetParent(ally.transform);
                    Destroy(healVFX, 1f);
                }
            }
        }
    }

    void ResetCastingState()
    {
        isCastingHeal = false; // Mở khóa chân di chuyển
        nextHealTime = Time.time + healRate; // Cài lịch cho lần tiếp theo
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
}