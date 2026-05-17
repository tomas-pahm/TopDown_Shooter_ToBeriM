using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Cấu hình chung")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public MuzzleFLashEffect muzzleFlash;
    public Transform armTransform;

    [Header("Thông số súng")]
    public float fireRate = 0.5f;       // Tốc độ bắn (Tiểu liên thì cho nhỏ thôi)
    public float recoilDistance = 0.1f; // Độ giật
    public int bulletsPerShot = 1;      // Số viên mỗi lần bắn (Lục/Tiểu liên = 1, Shotgun = 3-5)
    public float spreadAngle = 0f;  
    
    [HideInInspector] public float burnMultiplier = 1f;

    private float nextFireTime;
    private Vector3 initialArmPosition;

    public Rigidbody2D playerRb;

    void Start() {
        if (armTransform != null) initialArmPosition = armTransform.localPosition;
    }

    public void Fire() {
        if (Time.time < nextFireTime || bulletPrefab == null) return;

        // Xử lý bắn nhiều viên (Dùng cho Shotgun)
        for (int i = 0; i < bulletsPerShot; i++) {
            SpawnBullet();
        }

        // Hiệu ứng Visual
        if (muzzleFlash != null) muzzleFlash.Activate();
        HandleRecoil();

        nextFireTime = Time.time + (fireRate / burnMultiplier);
    }

    void SpawnBullet() {
        Debug.Log("Vận tốc Player hiện tại: " + playerRb.linearVelocity.magnitude);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.bulletDamage=Mathf.RoundToInt(b.bulletDamage*burnMultiplier);
        }
        // Tính hướng cơ bản
        Vector2 direction = (firePoint.position - transform.position).normalized;
        
        Vector2 pVelocity = playerRb != null ? playerRb.linearVelocity : Vector2.zero;

        // Nếu có độ tỏa (Spread), tính lại hướng
        if (spreadAngle > 0) {
            float randomSpread = Random.Range(-spreadAngle, spreadAngle);
            direction = Quaternion.Euler(0, 0, randomSpread) * direction;
        }

        bullet.GetComponent<Bullet>().Setup(direction, pVelocity);
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