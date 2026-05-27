using UnityEngine;



public class ThingsSpawner : MonoBehaviour

{

    public GameObject thingPrefab;



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


        InvokeRepeating("SpawnAThing", 1f, spawnTime);


    }



    void SpawnAThing()

    {

        if (thingPrefab == null || playerTransform == null)

            return;


        Vector2 randomDirection = Random.insideUnitCircle.normalized;


        float randomDistance = Random.Range(minRad, maxRad);


        Vector3 spawnPos = playerTransform.position + (Vector3)(randomDirection * randomDistance);

        spawnPos.z = 0f;


        Instantiate(thingPrefab, spawnPos, Quaternion.identity);

    }
}