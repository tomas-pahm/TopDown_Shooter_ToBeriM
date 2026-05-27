using UnityEngine;
using System.Collections;
using DefaultNamespace.Level;

public class EnemySpawner : MonoBehaviour
{
    [Header("=== CARD MÀN CHƠI HIỆN TẠI ===")]
    public LevelData currentLevelCard; // Kéo thả cái Card Màn Chơi vào đây nhe Chấn!

    [Header("Cấu hình khoảng cách quanh Player")]
    public float minRad = 5f;
    public float maxRad = 15f;
    
    private Transform playerTransform;
    [HideInInspector] public float levelTimer = 0f;
    private bool isLevelEnded = false;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        if (currentLevelCard == null)
        {
            Debug.LogError("🚨 Chấn ơi! Chưa kéo Card Màn Chơi vào EnemySpawner kìa ông nậu!");
            return;
        }
        
        foreach (WaveInfo wave in currentLevelCard.wavesPool)
        {
            StartCoroutine(SpawnWaveRoutine(wave));
        }
    }

    void Update()
    {
        if (isLevelEnded) return;
        
        levelTimer += Time.deltaTime;
        
        if (levelTimer >= currentLevelCard.levelDuration)
        {
            EndLevelAndSpawnBoss();
        }
    }
    
    IEnumerator SpawnWaveRoutine(WaveInfo wave)
    {
        // Chờ cho đến đúng số giây quy định
        yield return new WaitForSeconds(wave.startAtSecond);

        Debug.Log($"⚔️ Bắt đầu: {wave.waveName}!");

        int spawnedCount = 0;
        
        while (spawnedCount < wave.maxEnemiesInWave && !isLevelEnded)
        {
            if (wave.enemyPrefab != null && playerTransform != null)
            {
                SpawnAnEnemy(wave.enemyPrefab);
                spawnedCount++;
            }
            
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    void SpawnAnEnemy(GameObject prefab)
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minRad, maxRad);
        
        Vector3 spawnPos = playerTransform.position + (Vector3)(randomDirection * randomDistance);
        spawnPos.z = 0f;
        
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    void EndLevelAndSpawnBoss()
    {
        isLevelEnded = true;
        Debug.Log("🏆 Hết giờ màn chơi! Đang gọi Boss trùm cuối xuất hiện!");

        if (currentLevelCard.bossPrefab != null && playerTransform != null)
        {
            SpawnAnEnemy(currentLevelCard.bossPrefab);
        }
        
        this.enabled = false;
    }
    
    public float GetRemainingTime()
    {
        if(currentLevelCard == null) return 0f;
        
        float timeRemain = currentLevelCard.levelDuration - levelTimer;
        
        return Mathf.Max(0f, timeRemain);
    }
}