using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public FloatingJoystick joystick; 
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool facingRight = true;
    public Animator anim; 

    void Start() {
        rb = GetComponent<Rigidbody2D>();
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
    }

    void FixedUpdate() {
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void Flip() {
        facingRight = !facingRight;
        // Lật nhân vật bằng cách xoay trục Y 180 độ
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}