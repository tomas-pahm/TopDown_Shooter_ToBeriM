using System.Collections; // 🚨 BẮT BUỘC PHẢI CÓ DÒNG NÀY ĐỂ XÀI COROUTINE NHA ÔNG NẬU
using UnityEngine;

public class KatanaSlashWave : Weapon 
{
    [Header("Cấu hình Tiến Hóa Kiếm Khí")]
    public int wavesCount = 1;          
    public float spreadAngle = 0f;      
    public bool isRadialAttack = false; 

    [Header("Cấu hình Hiệu Ứng Tuần Tự")]
    public float delayBetweenWaves = 0.04f; 

    protected override void SpawnBullet() 
    {
        if (bulletPrefab == null) return;

        
        StartCoroutine(SpawnWavesSequentialRoutine());
    }

    // ⏳ HÀM BẤM GIỜ ĐẺ KIẾM KHÍ
    private IEnumerator SpawnWavesSequentialRoutine()
    {
        Vector2 pVelocity = playerRb != null ? playerRb.linearVelocity : Vector2.zero;

        for (int i = 0; i < wavesCount; i++)
        {
            
            if (firePoint == null) yield break;

            // Tính toán hướng bay 
            Vector2 baseDirection = (firePoint.position - transform.position).normalized;
            Vector2 finalDirection = baseDirection;

            if (isRadialAttack)
            {
                float angleOffset = (360f / wavesCount) * i;
                finalDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;
            }
            else if (wavesCount > 1 && spreadAngle > 0)
            {
                float fraction = (float)i / (wavesCount - 1);
                float angleOffset = Mathf.Lerp(-spreadAngle, spreadAngle, fraction);
                finalDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;
            }

            // Đẻ kiếm khí
            GameObject wave = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet waveScript = wave.GetComponent<Bullet>();
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayVFX(AudioManager.Instance.slash2Sound);
            }

            if (waveScript != null) 
            {
                float playerMult = playerCtrl != null ? playerCtrl.passiveDamageMultiplier : 1f;
                
                waveScript.bulletDamage = Mathf.RoundToInt(waveScript.bulletDamage * burnMultiplier * playerMult);
                if (wavesCount > 1)
                {
                    wave.transform.localScale *= (1f + (wavesCount * 0.15f)); 
                }
                waveScript.Setup(finalDirection, pVelocity);
            }

            // ⏱️ LỆNH KHÓA NHỊP
            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    public override void UpgradeStats(int level)
    {
        switch (level)
        {
            case 2:
                wavesCount = 2;
                spreadAngle = 15f;
                isRadialAttack = false;
                break;

            case 3:
                wavesCount = 3;
                spreadAngle = 30f;
                isRadialAttack = false;
                break;

            case 4:
                wavesCount = 4;
                spreadAngle = 45f; 
                isRadialAttack = false;
                break;
        }

        Debug.Log($"{gameObject.name} (Kiếm Khí) tiến hóa lên Lvl {level}!");
    }
}