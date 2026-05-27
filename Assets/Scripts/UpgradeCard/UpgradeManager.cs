using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance; // Hệ Singleton

    [Header("Cấu hình Thẻ Bài")]
    public GameObject levelUpPanel;       
    public GameObject upgradeCardPrefab;
    public UpgradeData[] allUpgradesPool;

    // Sổ ghi nhớ cấp độ dòng họ vũ khí
    private Dictionary<string, int> upgradeLevels = new Dictionary<string, int>();
    private PlayerController player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>(); 
        
        // --- NHẬT KÝ ĐẦU TRẬN ---
        // ⚔️ Hệ Vũ khí Active
        upgradeLevels["Shotgun"] = 0;
        upgradeLevels["Katana"] = 0;
        upgradeLevels["VoidSword"] = 0;

        if (player != null && player.curWeapon != null)
        {
            string currentEquippedWeapon = player.curWeapon.weaponRegistryName;

            if (upgradeLevels.ContainsKey(currentEquippedWeapon))
            {
                upgradeLevels[currentEquippedWeapon] = 1;
            }
        }
        
        upgradeLevels["Passive_Cooldown"] = 0;
        upgradeLevels["Passive_MaxHealth"] = 0;
        upgradeLevels["Passive_Speed"] = 0;
        upgradeLevels["Passive_Damage"] = 0;
    }

    public void OpenUpgradeMenu()
    {
        Time.timeScale = 0f; 

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
            foreach (Transform child in levelUpPanel.transform) { Destroy(child.gameObject); }
        }

        if (allUpgradesPool != null && allUpgradesPool.Length > 0)
        {
            List<UpgradeData> validUpgrades = new List<UpgradeData>();

            foreach (UpgradeData data in allUpgradesPool)
            {
                if (data == null) continue;

                int currentWeaponLvl = 0;
                if (upgradeLevels.ContainsKey(data.targetWeaponName))
                {
                    currentWeaponLvl = upgradeLevels[data.targetWeaponName];
                }
                
                if (data.starLevel == currentWeaponLvl + 1)
                {
                    validUpgrades.Add(data);
                }
            }

           
            int cardsSpawned = 0;

            while (validUpgrades.Count > 0 && cardsSpawned < 3)
            {
                int randomIndex = Random.Range(0, validUpgrades.Count);
                UpgradeData selectedData = validUpgrades[randomIndex];
                validUpgrades.RemoveAt(randomIndex); 

                if (upgradeCardPrefab != null && levelUpPanel != null)
                {
                    GameObject cardObj = Instantiate(upgradeCardPrefab, levelUpPanel.transform);
                    UpgradeCard cardScript = cardObj.GetComponent<UpgradeCard>();

                    if (cardScript != null)
                    {
                        cardScript.SetupCard(selectedData.icon, selectedData.description, selectedData.starLevel, () =>
                        {
                            ExecuteUpgrade(selectedData);
                            upgradeLevels[selectedData.targetWeaponName] = selectedData.starLevel;
                            Debug.Log($"[Tiến Cấp Hệ] {selectedData.targetWeaponName} đã tăng lên Level {selectedData.starLevel}!");
                        });
                        cardsSpawned++;
                    }
                }
            }
            
            if (cardsSpawned == 0)
            {
                Debug.LogWarning("⚠️ Không còn thẻ nào hợp lệ để nâng cấp! Tự động chạy tiếp game.");
                Time.timeScale = 1f;
                if (levelUpPanel != null) levelUpPanel.SetActive(false);
            }
        }
    }

    void ExecuteUpgrade(UpgradeData data)
    {
        Debug.Log("Player đã chọn thẻ: " + data.upgradeName);

        switch (data.type)
        {
            case UpgradeType.AddNewWeapon:
                if (data.weaponPrefab != null)
                {
                    GameObject newWeapon = Instantiate(data.weaponPrefab, player.transform.position, Quaternion.identity);
                    newWeapon.transform.SetParent(player.transform); 
                    newWeapon.name = data.targetWeaponName;
                    newWeapon.transform.localPosition = Vector3.zero;
                    newWeapon.transform.localScale = Vector3.one; 
                }
                break;

            case UpgradeType.LevelUpWeapon:
                Transform handTransformToUpgrade = player.transform.Find("ArmPivot/Hand");
                if (handTransformToUpgrade != null)
                {
                    Transform weaponToUpgrade = handTransformToUpgrade.Find(data.targetWeaponName);
                    if (weaponToUpgrade != null)
                    {
                        Weapon weaponScript = weaponToUpgrade.GetComponent<Weapon>();
                        if (weaponScript != null)
                        {
                            weaponScript.UpgradeStats(data.starLevel); 
                        }
                    }
                }
                break;

            case UpgradeType.SwapWeapon:
                Transform handTransform = player.transform.Find("ArmPivot/Hand");
                if (handTransform != null)
                {
                    
                    foreach (Transform child in handTransform)
                    {
                        if (child.name != "FirePoint") 
                        {
                            string oldWeaponName = child.name; 
                            
                            if (oldWeaponName != data.targetWeaponName && upgradeLevels.ContainsKey(oldWeaponName))
                            {
                                upgradeLevels[oldWeaponName] = 0;
                                Debug.Log($"[Reset] Đổi hệ chiến đấu! Hạ hệ '{oldWeaponName}' về Level 0 thành công.");
                            }
                            
                            Destroy(child.gameObject); 
                        }
                    }
                    
                    if (data.weaponPrefab != null)
                    {
                        GameObject newWeapon = Instantiate(data.weaponPrefab, handTransform.position, handTransform.rotation);
                        newWeapon.transform.SetParent(handTransform);
                        
                        newWeapon.transform.localPosition = data.weaponPrefab.transform.localPosition;
                        newWeapon.transform.localRotation = data.weaponPrefab.transform.localRotation;
                        newWeapon.transform.localScale = data.weaponPrefab.transform.localScale;
                        
                        newWeapon.name = data.targetWeaponName; 

                        Weapon newWeaponScript = newWeapon.GetComponent<Weapon>();
                        if (newWeaponScript != null)
                        {
                            newWeaponScript.playerRb = player.GetComponent<Rigidbody2D>();
                            newWeaponScript.armTransform = player.transform.Find("ArmPivot");
                        }
                        player.curWeapon = newWeaponScript; 
                    }
                }
                break;
            
            case UpgradeType.PassiveBuff:
                if (player != null)
                {
                    player.ApplyPassiveBuff(data.passiveType, data.passiveValue);
                }
                break;
        }

        if (levelUpPanel != null) levelUpPanel.SetActive(false); 
        Time.timeScale = 1f;                                   
    }
}