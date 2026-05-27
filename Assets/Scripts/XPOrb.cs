using UnityEngine;

namespace DefaultNamespace
{
    public class XPOrb : MonoBehaviour
    {
        public float xpAmount;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" && collision.isTrigger)
            {
                PlayerController player = collision.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    player.GainXP(xpAmount); // Gọi hàm cộng XP của Player
                    Destroy(gameObject);
                }
            }
        }
    }
}