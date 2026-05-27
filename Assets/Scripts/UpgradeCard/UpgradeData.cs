using UnityEngine;

// Định nghĩa các loại chức năng của thẻ bài
public enum UpgradeType
{
    AddNewWeapon,    // Thêm vũ khí mới hoàn toàn (Drone, Gạch, Molly)
    LevelUpWeapon,   // Tăng cấp/Tăng sao cho vũ khí hiện tại
    SwapWeapon,
    PassiveBuff       // Đổi vũ khí cũ lấy vũ khí mới (Ví dụ: Shotgun -> Kiếm)
}

public enum PassiveBuffType
{
    CooldownReduction,
    MovementSpeed,
    MaxHealth,
    DamageBoost
}

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Roguelike/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("Giao diện hiển thị")]
    public string upgradeName;       
    public Sprite icon;             
    [TextArea(2, 4)]
    public string description;       
    public int starLevel;            

    [Header("Logic Chức Năng")]
    public UpgradeType type;         
    public GameObject weaponPrefab; 
    public string targetWeaponName; 
    
    [Header("=== CẤU HÌNH THEO LOẠI PASSIVE ===")]
    public PassiveBuffType passiveType; 
    public float passiveValue;
}