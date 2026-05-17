using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float spawnTime = 1f;

    [Header("Cấu hình khoảng cạch quanh Player")]
    public float minRad = 5f;
    public float maxRad = 15f;
    
    private Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        InvokeRepeating("SpawnAnEnemy", 1f, spawnTime);
        
    }

    void SpawnAnEnemy()
    {
        if (enemyPrefab == null || playerTransform == null)
            return;
        
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        
        float randomDistance = Random.Range(minRad, maxRad);
        
        Vector3 spawnPos = playerTransform.position + (Vector3)(randomDirection * randomDistance);
        spawnPos.z = 0f;
        
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
