using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Cấu hình chung")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public MuzzleFLashEffect muzzleFlash;
    public Transform armTransform;

    [Header("Thông số súng")]
    public float fireRate = 0.5f;       
    public float recoilDistance = 0.1f; 
    public int bulletsPerShot = 1;      
    public float spreadAngle = 0f;  
    
    [HideInInspector] public float burnMultiplier = 1f;

    private float nextFireTime;
    private Vector3 initialArmPosition;

    public Rigidbody2D playerRb;

    void Start() {
        if (armTransform != null) initialArmPosition = armTransform.localPosition;
    }

    public void Fire() {
        // Sử dụng Time.time để check hồi chiêu đạn
        if (Time.time < nextFireTime || bulletPrefab == null) return;

        for (int i = 0; i < bulletsPerShot; i++) {
            SpawnBullet();
        }

        if (muzzleFlash != null) muzzleFlash.Activate();
        HandleRecoil();

        // Tốc độ bắn chuẩn bài: Điên càng nặng (burnMultiplier càng to) thì thời gian chờ hồi đạn càng nhỏ -> Bắn càng nhanh!
        nextFireTime = Time.time + (fireRate / burnMultiplier);
    }

    void SpawnBullet() {
        if (playerRb != null)
        {
            Debug.Log("Vận tốc Player hiện tại: " + playerRb.linearVelocity.magnitude);
        }

        // 1. Sinh ra viên đạn
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // 2. CHỈ GỌI GETCOMPONENT ĐÚNG 1 LẦN DUY NHẤT ĐỂ TIẾT KIỆM RAM
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            // Tính toán sát thương tăng thêm dựa trên kỹ năng điên BurnSkill
            // Ép kiểu sang int chuẩn chỉnh không lo lỗi float
            bulletScript.bulletDamage = Mathf.RoundToInt(bulletScript.bulletDamage * burnMultiplier);
            
            // Tính hướng bay của đạn
            Vector2 direction = (firePoint.position - transform.position).normalized;
            
            // Lấy vận tốc hiện tại của Player ném vào đạn
            Vector2 pVelocity = playerRb != null ? playerRb.linearVelocity : Vector2.zero;

            // Nếu súng có độ tỏa (như Shotgun) thì bẻ hướng đạn
            if (spreadAngle > 0) {
                float randomSpread = Random.Range(-spreadAngle, spreadAngle);
                direction = Quaternion.Euler(0, 0, randomSpread) * direction;
            }

            // Gọi hàm Setup truyền hướng và vận tốc vào đạn thông qua biến script đã tìm ở trên
            bulletScript.Setup(direction, pVelocity);
        }
    }

    void HandleRecoil() {
        if (armTransform != null) {
            armTransform.localPosition = initialArmPosition - new Vector3(recoilDistance, 0, 0);
            CancelInvoke("ResetArmPosition");
            Invoke("ResetArmPosition", 0.05f);
        }
    }

    void ResetArmPosition() {
        if (armTransform != null) armTransform.localPosition = initialArmPosition;
    }
}