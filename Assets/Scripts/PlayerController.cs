using System.Collections;
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
    [Header("Dash Skill")]
    public float dashTime = 0.2f;
    private float _dashTime;
    public float dashSpeedMutliplier = 1.5f;
    private bool isDashing;
    public GameObject glitchEffect;
    public float delayGlitchSeconds = 0.05f;
    private Coroutine dashEffectCoroutine;
    

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

        if (Input.GetKeyDown(KeyCode.Space) && _dashTime <= 0 && !isDashing)
        {
            _dashTime = dashTime;
            isDashing = true;
            StartDashEffect();
        }

        // Chỉ đếm ngược khi đang lướt
        if (isDashing)
        {
            _dashTime -= Time.deltaTime;
            if (_dashTime <= 0)
            {
                isDashing = false;
                StopDashEffect();// Hết thời gian lướt thì tắt trạng thái
            }
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
        float currentSpeed = moveSpeed; 

        // Nếu đang Dash, lấy tốc độ hiện tại nhân thêm hệ số lướt
        if (isDashing)
        {
            currentSpeed = moveSpeed * dashSpeedMutliplier;
        }

        // Áp vận tốc vật lý cực kỳ an toàn
        rb.linearVelocity = moveInput.normalized * currentSpeed;
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

   void StopDashEffect()
   {
       if (dashEffectCoroutine != null)
           StopCoroutine(dashEffectCoroutine);
       
   }
   void StartDashEffect()
   {
       if (dashEffectCoroutine != null)
           StopCoroutine(dashEffectCoroutine);
           dashEffectCoroutine = StartCoroutine(DashEffectCoroutine());
       
   }

   IEnumerator DashEffectCoroutine()
   {
       while (true)
       {
           GameObject glitch = Instantiate(glitchEffect, transform.position, transform.rotation);
           Destroy(glitch, 0.3f);
           yield return new WaitForSeconds(delayGlitchSeconds);
       }
   }
}