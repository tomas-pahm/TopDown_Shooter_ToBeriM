using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f;
    public int bulletDamage = 1;
    public float bulletLifeTime = 2f;

    [Header("=== CẤU HÌNH XOAY (DÙNG CHO KIẾM KHÍ) ===")]
    public bool canRotate = false;    
    public float rotationSpeed = 720f;
    
    [Header("Có xuyên vật thể không")]
    public bool isNotThrough;

    private Rigidbody2D rb;
    private Dictionary<Collider2D, float> hitCooldowns = new Dictionary<Collider2D, float>();
    private float multiHitDelay = 0.08f;

    public void Setup(Vector2 direction, Vector2 playerVelocity)
    {
        rb = GetComponent<Rigidbody2D>();
        
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 

        rb.linearVelocity = (direction * bulletSpeed) + playerVelocity;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); 
    }

    void Start()
    {
        Destroy(gameObject, bulletLifeTime);
    }

    void Update()
    {
        // 🔥 KIỂM TRA CÔNG TẮC: Thằng nào bật ON (true) thì mới cho xoay tít thò lò!
        if (canRotate)
        {
            float curSpeed = rb != null ? rb.linearVelocity.magnitude : bulletSpeed;
            
            float speedFactor = bulletSpeed > 0 ? (curSpeed/bulletSpeed) : 1f;

            float dynamicRotation = speedFactor > 1f ? speedFactor * rotationSpeed : rotationSpeed;
            
            transform.Rotate(0, 0, dynamicRotation * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (hitCooldowns.ContainsKey(collision))
            {

                if (Time.time < hitCooldowns[collision]) return;

                hitCooldowns[collision] = Time.time + multiHitDelay;
            }
            else
            {
                hitCooldowns.Add(collision, Time.time + multiHitDelay);
            }
            damageable.TakeDamage(bulletDamage);
            Debug.Log($"🎯 Kiếm khí xoay cứa trúng: {collision.name} | Sát thương: {bulletDamage}");
            
            if (isNotThrough) Destroy(gameObject); 
        }
    }
}
