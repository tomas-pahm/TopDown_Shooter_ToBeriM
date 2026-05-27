using UnityEngine;

public class KatanaExplosion : Weapon 
{
    [Header("Cấu hình cận chiến Katana")]
    public int weaponDamage = 20; 
    public float attackRange = 1.5f; 
    public LayerMask targetLayer; 

    [Header("Cấu hình Kích Thước Visual")]
    public float effectScale = 1f;  // Hệ số phóng to hình ảnh (Level 1 mặc định là 1)

    protected override void SpawnBullet() 
    {
        if (bulletPrefab != null) {
            GameObject fx = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            
            Vector3 fxScale = fx.transform.localScale * effectScale;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayVFX(AudioManager.Instance.slash1Sound);
            }

            float facingDirection = (transform.lossyScale.x > 0) ? 1 : -1;
            fxScale.x *= facingDirection; 
            
            fx.transform.localScale = fxScale;
            
            Destroy(fx, 0.5f); 
        }
        float playerMult = playerCtrl != null ? playerCtrl.passiveDamageMultiplier : 1f;
        
        int finalDamage = Mathf.RoundToInt(weaponDamage * burnMultiplier * playerMult);
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(firePoint.position, attackRange, targetLayer);
        
        foreach (Collider2D enemy in hitEnemies) 
        {
            if (!enemy.isTrigger) continue;
            
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(finalDamage); 
                Debug.Log($"Kiếm khí khổng lồ chém trúng {enemy.name}! Gây {finalDamage} sát thương.");
            }
        }
    }
    
    public override void UpgradeStats(int level) 
    {
        switch (level)
        {
            case 2:
                weaponDamage = 30;       
                attackRange = 2.5f;     
                effectScale = 1.2f;      
                break;

            case 3:
                weaponDamage = 45;       
                attackRange = 3f;     
                effectScale = 1.8f;      
                break;

            case 4:
                weaponDamage = 65;      
                attackRange = 4.5f;      
                effectScale = 2.4f;      
                break;
        }
        Debug.Log($"{gameObject.name} (Katana) đã tiến hóa lên Cấp {level}! Range: {attackRange}, Scale: {effectScale}");
    }
    
    void OnDrawGizmosSelected() 
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, attackRange);
        }
    }
}