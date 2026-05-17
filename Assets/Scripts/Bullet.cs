using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f;
    public int bulletDamage = 1;
    public float bulletLifeTime = 2f;

    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Setup(Vector2 direction, Vector2 playerVelocity)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Ép vận tốc bay theo hướng súng vừa đưa, bất kể Scale của Player là bao nhiêu
        rb.linearVelocity = (direction * bulletSpeed) +playerVelocity;
        
        Debug.Log("Speed Bullet: "+rb.linearVelocity.magnitude);

        // XOAY SPRITE: Để đạn nằm ngang theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Nếu đạn bị dọc, dùng -90. Nếu đạn chuẩn ngang thì bỏ -90 đi.
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); 
    }

    void Start()
    {
        Destroy(gameObject, bulletLifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem có trúng Enemy không
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(bulletDamage); 
            }
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
