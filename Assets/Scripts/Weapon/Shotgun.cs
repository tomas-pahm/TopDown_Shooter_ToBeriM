using UnityEngine;

public class Shotgun : Weapon
{
    [Header("Cấu hình đặc thù Shotgun (Lớp Con)")]
    public int bulletsPerShot = 1;      
    public float spreadAngle = 0f;  
    
    protected override void SpawnBullet()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            if (bulletPrefab == null) return; 

            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            
            if (bulletScript != null)
            {
                float playerMult = playerCtrl != null ? playerCtrl.passiveDamageMultiplier : 1f;
                
                bulletScript.bulletDamage = Mathf.RoundToInt(bulletScript.bulletDamage * burnMultiplier * playerMult);
                
                Vector2 direction = (firePoint.position - transform.position).normalized;
                
                Vector2 pVelocity = playerRb != null ? playerRb.linearVelocity : Vector2.zero;
                
                if (spreadAngle > 0) {
                    float randomSpread = Random.Range(-spreadAngle, spreadAngle);
                    direction = Quaternion.Euler(0, 0, randomSpread) * direction;
                }
                
                bulletScript.Setup(direction, pVelocity);
            }
        }
    }
    
    public override void UpgradeStats(int level)
    {
        switch (level)
        {
            case 2:
                bulletsPerShot = 5;     
                spreadAngle = 15f;      
                break;
            case 3:
                bulletsPerShot = 7;      
                spreadAngle = 12f;
                break;
            case 4:
                bulletsPerShot = 9;
                spreadAngle = 10f;
                break;
        }
        Debug.Log($"{gameObject.name} (Shotgun) đã đập Stats thành công lên Level {level}!");
    }
}