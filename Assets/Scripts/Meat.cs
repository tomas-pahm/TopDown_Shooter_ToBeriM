using UnityEngine;

namespace DefaultNamespace
{
    public class Meat : MonoBehaviour
    {
        private bool isCollected = false;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Kiểm tra xem có phải Player chạm vào không
            if (collision.CompareTag("Player") && collision.isTrigger)
            {
                if(isCollected == true) return;
                PlayerController player = collision.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    // Gọi hàm hồi máu của ông (Ví dụ truyền vào giá trị âm để trừ ngược lại trong hàm TakeDamage)
                    // Hoặc nếu ông có hàm Heal() thì gọi player.Heal(20);
                    player.TakeDamage(-20); 
                    
                    isCollected = true;

                    // Hồi máu xong thì xóa miếng thịt đi
                    Destroy(gameObject);
                }
            }
        }
    }
}