using UnityEngine;
using UnityEngine.UI;

public class BurnSkill : MonoBehaviour
{
    [Header("Liên kết thành phần")]
    private PlayerController player;
    private Weapon weapon;

    [Header("WhiteAsh")] public GameObject whiteAshEffect;
        
    public Slider cooldownSlider;
    
    [Header("Cấu hình skill")]
    public bool isBurning = false;
    public float burnDurationToMax = 20f;
    public float maxCooldown = 20f;
    public float burnCooldown = 0f;
    private float currentBurnTime = 0f;
    public float sacrificeMultiplier = 2f;

    private float burnHpTimer = 0f;

    private float originalMoveSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>() ?? GetComponentInParent<PlayerController>();
        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.GetComponent<PlayerController>();
        }

        if (player != null)
            originalMoveSpeed = player.moveSpeed;

        if (whiteAshEffect != null)
        {
            whiteAshEffect.SetActive(false);
        }

        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = maxCooldown;
            cooldownSlider.value = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && burnCooldown <= 0f)
        {
            toggleBurn();
        }

        SkillCoolDown();
        
        HandleBurnLogic();

        UpdateCooldownUI();
    }

    public void toggleBurn()
    {
        // Bỏ qua nếu player thực sự bằng null (nhưng code trên đã tìm chắc chắn có rồi)
        if (player == null || player.curHealth <= 0) 
        {
            return; 
        }

        isBurning = !isBurning; //
        Debug.Log($"🔥 TRẠNG THÁI BURN THAY ĐỔI: {isBurning}");

        if (whiteAshEffect != null)
        {
            whiteAshEffect.SetActive(isBurning); //
        }

        if (!isBurning)
        {
            ResetStats(); //
            burnCooldown = maxCooldown; //
        }
    }

    void SkillCoolDown()
    {
        if (isBurning) return;

        if (burnCooldown > 0f)
        {
            burnCooldown -= Time.deltaTime;
        }
        else burnCooldown = 0f;
    }

    void UpdateCooldownUI()
    {
        if (cooldownSlider == null) return;

        if (isBurning)
        {
            cooldownSlider.maxValue = maxCooldown;
        }
        else
        {
            cooldownSlider.value = burnCooldown;
        }
    }

    void HandleBurnLogic()
    {
        if (isBurning)
        {
            burnHpTimer += Time.deltaTime;
            if (burnHpTimer >= 1f)
            {
                player.TakeDamage(1f);
                burnHpTimer = 0f;
            }
            currentBurnTime += Time.deltaTime;
            if (currentBurnTime >= burnDurationToMax) currentBurnTime = burnDurationToMax;
        
            float progress = currentBurnTime / burnDurationToMax;
        
            player.moveSpeed = Mathf.Lerp(originalMoveSpeed, originalMoveSpeed * sacrificeMultiplier, progress);

            // 🎯 RADAR QUÉT THỜI GIAN THỰC:
            // Vũ khí mới vừa Instantiate ra là dính đòn ngay lập tức, đéo chạy đường trời nào được!
            Weapon activeWeapon = GetComponentInChildren<Weapon>(); 
        
            if (activeWeapon != null)
            {
                activeWeapon.burnMultiplier = Mathf.Lerp(1f, sacrificeMultiplier, progress);
            }
        }
    }

    void ResetStats()
    {
        currentBurnTime = 0f;
        burnHpTimer = 0f;
        if (player != null) player.moveSpeed = originalMoveSpeed;
        
        Weapon activeWeapon = GetComponentInChildren<Weapon>();
        if (activeWeapon != null)
        {
            activeWeapon.burnMultiplier = 1f;
        }
    }
    
    void OnDisable()
    {
        if (whiteAshEffect != null) whiteAshEffect.SetActive(false);
        ResetStats();
    }
}
