using System.Collections;
using UnityEngine;

public class SheepAI : MonoBehaviour, IDamageable
{
    private Animator anim;
    private Rigidbody2D rb;
    public float moveSpeed = 1.5f;
    private Vector2 moveDirection;
    private bool isDead = false;
    
    [Header("Cấu hình thịt")]
    public GameObject meatPrefab;

    public float health = 10f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(SheepBehaviorCoroutine());
    }

    public void TakeDamage(int damage)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVFXWithDuration(AudioManager.Instance.sheepHitSound, 1f);
        }
        health -= damage;
        if (health <= 0f)
            Die();
    }

    void Die()
    {
        if(isDead) return;
        isDead = true;
        if (meatPrefab != null)
        {
            Instantiate(meatPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void FixedUpdate()
    {
       
        if (anim.GetInteger("actionID") == 2)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.zero; 
        }
    }

    IEnumerator SheepBehaviorCoroutine()
    {
        while (true)
        {
            int randomAction = Random.Range(0, 3); 
            anim.SetInteger("actionID", randomAction);

            if (randomAction == 2)
            {
                moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                
                if (moveDirection.x != 0)
                {
                    transform.localScale = new Vector3(moveDirection.x > 0 ? 1 : -1, 1, 1);
                }
            }

            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }
}