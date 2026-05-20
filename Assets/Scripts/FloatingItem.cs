using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("Cấu hình nhấp nhô")]
    public float floatSpeed = 3f;       // Tốc độ bay lên xuống nhanh hay chậm
    public float floatAmplitude = 0.15f; // Độ cao bay nhấp nhô (đừng để to quá nó bay mất xác)

    private Vector3 startPos;

    void Start()
    {
        // Lưu lại vị trí gốc lúc miếng thịt mới xuất hiện
        startPos = transform.position;
    }

    void Update()
    {
        // Công thức Sin thần thánh để tạo chuyển động mượt mà
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        
        // Cập nhật lại vị trí mới cho miếng thịt
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}