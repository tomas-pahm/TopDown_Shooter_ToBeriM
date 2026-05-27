using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("WEAPON IDENTIFIERS")] public string weaponRegistryName;
    
    [Header("Cấu hình chung (Lớp Cha)")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public MuzzleFLashEffect muzzleFlash;
    public Transform armTransform;

    [Header("Thông số cơ bản")]
    public float fireRate = 0.5f;       
    public float recoilDistance = 0.1f; 
    
    [HideInInspector] public float burnMultiplier = 1f;
     public Rigidbody2D playerRb; 
    [HideInInspector] public PlayerController playerCtrl;

    protected Vector3 initialArmPosition; // Xóa bỏ biến nextFireTime ở đây

    protected virtual void Start() {
        if (armTransform != null) initialArmPosition = armTransform.localPosition;

        if (playerRb == null)
        {
            playerRb = GetComponentInParent<Rigidbody2D>();
        }

        if (playerCtrl == null)
        {
            playerCtrl = GetComponentInParent<PlayerController>();
        }
    }

    // 🔥 HÀM FIRE SIÊU SẠCH: Gọi là bắn, đéo lo đếm giờ!
    public virtual void Fire() {
        SpawnBullet();

        if (muzzleFlash != null) muzzleFlash.Activate();
        HandleRecoil();
    }

    protected abstract void SpawnBullet();
    public abstract void UpgradeStats(int level);

    protected void HandleRecoil() {
        if (armTransform != null) {
            armTransform.localPosition = initialArmPosition - new Vector3(recoilDistance, 0, 0);
            CancelInvoke("ResetArmPosition");
            Invoke("ResetArmPosition", 0.05f);
        }
    }

    protected void ResetArmPosition() {
        if (armTransform != null) armTransform.localPosition = initialArmPosition;
    }
}