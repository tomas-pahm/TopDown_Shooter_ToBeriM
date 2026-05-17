using UnityEngine;

public class ArmRotation : MonoBehaviour
{
    public FloatingJoystick joystick;
    public float rotationSpeed = 15f;

    [Header("Bobbing Settings")]
    public float bobSpeed = 10f;     // Tốc độ nhấp nhô
    public float bobAmount = 0.05f;  // Độ cao nhấp nhô (đừng để quá lớn, sẽ bị rời vai)
    
    private Vector2 direction;
    private Transform playerTransform;
    private Vector3 initialPosition; // Vị trí gốc của ArmPivot

    void Start()
    {
        playerTransform = transform.parent;
        initialPosition = transform.localPosition; 
    }

    void Update()
    {
        // 1. Lấy input (như cũ)
        if (joystick != null && (joystick.Horizontal != 0 || joystick.Vertical != 0))
            direction = new Vector2(joystick.Horizontal, joystick.Vertical);
        else
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // 2. Xử lý xoay (có Fix Flip như nãy)
        HandleRotation();

        // 3. Xử lý nhấp nhô (Bobbing)
        HandleBobbing();
    }

    void HandleRotation()
    {
        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (playerTransform.localScale.x < 0) targetAngle += 180f;

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleBobbing()
    {
        if (direction.magnitude > 0.1f) // Chỉ nhấp nhô khi đang di chuyển
        {
            // Tính toán vị trí Y mới dựa trên hàm Sin
            float newY = initialPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.localPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
        }
        else // Khi đứng yên thì quay về vị trí gốc mượt mà
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * bobSpeed);
        }
    }
}