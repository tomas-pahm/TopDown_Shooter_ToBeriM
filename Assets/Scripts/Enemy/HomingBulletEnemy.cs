using UnityEngine;

public class HomingBulletEnemy : Enemy // Kế thừa từ class Enemy của Chấn
{
    [Header("Cấu hình tên lửa uốn lượn")]
    public float rotateSpeed = 200f; // Tốc độ bẻ lái của viên đạn (càng cao dí càng gắt)

    protected override void Start()
    {
        base.Start(); // Bú sạch đống tìm Player, Rigidbody của cha
        isItBullet = true; // Ép nó luôn là đạn ngoài code luôn cho chắc
    }

    protected override void FixedUpdate()
    {
        if (isDead || player == null) return;
        
        Vector2 direction = (Vector2)player.position - rb.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        
        rb.linearVelocity = transform.up * moveSpeed;
    }
}