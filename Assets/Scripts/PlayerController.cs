using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    
    public FloatingJoystick joystick; 
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool facingRight = true;
    public Animator anim;
    public Weapon curWeapon;
    [Header("Chỉ số Player")]
    public float maxHealth = 100;
    public float curHealth;
    public float moveSpeed = 15f;
    [Header("UI Thanh Máu")]
    public Slider healthSlider;
    private bool isDead = false;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        curHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = curHealth;
        }
    }

    void Update() {
        // Lấy input (Sau khi ông đã chỉnh Active Input Handling thành Both)
        if (joystick != null && (joystick.Horizontal != 0 || joystick.Vertical != 0)) {
            moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        } else {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        // Kiểm tra hướng để lật mặt
        if (moveInput.x > 0 && !facingRight) {
            Flip();
        } else if (moveInput.x < 0 && facingRight) {
            Flip();
        }
        
        bool isWalking = moveInput.magnitude > 0;
        anim.SetBool("isWalking", isWalking);
        
        if (Input.GetButtonDown("Fire1"))
        {
            curWeapon.Fire();
        }
    }

    void FixedUpdate() // Di chuyển vật lý nên để trong FixedUpdate
    {
        // Giả sử direction là Vector2 từ Input của ông
        rb.linearVelocity = moveInput.normalized * moveSpeed; 
    }

    void Flip() {
        facingRight = !facingRight;
        // Lật nhân vật bằng cách xoay trục Y 180 độ
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
   public void TakeDamage(float damage)
    {
        if(isDead) return;
        curHealth -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = curHealth;
        }
        Debug.Log("Máu Player còn: " + curHealth);
        if (curHealth <= 0)
        {
            PlayerDie();
        }
    }
   
   void PlayerDie(){
        isDead = true;
        anim.SetTrigger("isDead");
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        Destroy(gameObject, 0.75f);
   }
}