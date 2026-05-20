using System.Collections;
using UnityEngine;

public class DuckAI : MonoBehaviour
{
    private Rigidbody2D rb;
    public float swimSpeed = 1f; 
    private Vector2 swimDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        StartCoroutine(DuckSwimCoroutine());
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + swimDirection * swimSpeed * Time.fixedDeltaTime);
    }

    IEnumerator DuckSwimCoroutine()
    {
        while (true)
        {
            swimDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            
            if (swimDirection.x != 0)
            {
                transform.localScale = new Vector3(swimDirection.x > 0 ? 1 : -1, 1, 1);
            }
            
            float swimTime = Random.Range(2f, 5f);
            yield return new WaitForSeconds(swimTime);
            
            swimDirection = Vector2.zero;
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
    }
}