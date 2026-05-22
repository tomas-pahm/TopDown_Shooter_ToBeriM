using UnityEngine;

namespace DefaultNamespace
{
    public class Meat : MonoBehaviour
    {
        private bool isCollected = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
           
            if (isCollected) return; 
            
            if (collision.CompareTag("Player") && collision.isTrigger)
            {
                PlayerController player = collision.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    isCollected = true;

                    player.TakeDamage(-20); 
                    Destroy(gameObject);
                }
            }
        }
    }
}