using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Điều khiển")]
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

    [Header("Exp & Level")] 
    public float curExp;
    public int curLvl;
    public float expToNextLvl;
    public Slider expSlider;
    public TMPro.TMP_Text levelText;
    
    [Header("=== CẤU HÌNH AUTO FIRE (VAMPIRE STYLE) ===")]
    public Slider fireRateSlider;
    private float shotTimer;
    
    [Header("=== HỆ THỐNG CHỈ SỐ PASSIVE ===")]
    public float passiveCooldownReduction = 1f; 
    public float passiveMoveSpeedMultiplier = 1f; 
    public float passiveDamageMultiplier = 1f;
    public float passiveMaxHealthMultiplier = 1f;

    [Header("Nút Bấm")] public Animator dashBtnAnim;
    public Animator burnBtnAnim;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        
        int hpLvl = PlayerPrefs.GetInt("STAT_HP_LEVEL", 1);   
        int atkLvl = PlayerPrefs.GetInt("STAT_ATK_LEVEL", 1); 
        int cdLvl = PlayerPrefs.GetInt("STAT_CDR_LEVEL", 1);
        int speedLvl = PlayerPrefs.GetInt("STAT_SPEED_LEVEL", 1);
        
        maxHealth = 100f + (hpLvl - 1) * 25f; 
        curHealth = maxHealth;

        passiveCooldownReduction = 1f - (cdLvl - 1) * 0.05f;

        passiveMoveSpeedMultiplier = 1f + (speedLvl - 1) * 0.05f;
        
        passiveDamageMultiplier = 1f + (atkLvl - 1) * 0.15f; 
        
        if (healthSlider != null) { healthSlider.maxValue = maxHealth; healthSlider.value = curHealth; }
        if (expSlider != null) { expSlider.maxValue = expToNextLvl; expSlider.value = curExp; }
        if (levelText != null) { levelText.text = "LV. " + curLvl.ToString(); }
    
        Debug.Log($"[ĐACS3] Vào trận thành công! Máu hiện tại đạt: {maxHealth} (Level Shop: {hpLvl})");
    }

    void Update() {
        if (joystick != null && (joystick.Horizontal != 0 || joystick.Vertical != 0)) {
            moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        } else {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        if (Input.GetKeyDown(KeyCode.Space) && _dashTime <= 0 && !isDashing) {
            _dashTime = dashTime; isDashing = true; StartDashEffect();
        }

        if (isDashing) {
            _dashTime -= Time.deltaTime;
            if (_dashTime <= 0) { isDashing = false; StopDashEffect(); }
        }

        if (moveInput.x > 0 && !facingRight) Flip();
        else if (moveInput.x < 0 && facingRight) Flip();
        
        anim.SetBool("isWalking", moveInput.magnitude > 0);

        // Kiểm tra lên cấp
        if (curExp >= expToNextLvl) LevelUp();

        if (expSlider != null) { expSlider.maxValue = expToNextLvl; expSlider.value = curExp; }
        
        HandleAutoFire();
    }

    void FixedUpdate() {
        
        float currentSpeed = isDashing ? moveSpeed * dashSpeedMutliplier : moveSpeed;
        
        currentSpeed *= passiveMoveSpeedMultiplier; 

        rb.linearVelocity = moveInput.normalized * currentSpeed;
    }

    void Flip() {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
    void HandleAutoFire()
    {
        if (curWeapon == null) return;

        
        float passiveModifier = Mathf.Max(0.2f, passiveCooldownReduction);
        float actualCooldown = (curWeapon.fireRate / curWeapon.burnMultiplier) * passiveModifier;
        
        if (fireRateSlider != null)
        {
            fireRateSlider.maxValue = actualCooldown; 
            fireRateSlider.value = shotTimer;         
        }

        // Tích lũy thời gian trôi qua
        shotTimer += Time.deltaTime;

        
        if (shotTimer >= actualCooldown)
        {
            curWeapon.Fire(); 
            shotTimer = 0f;   
        }
    }
    
    public void TakeDamage(float damage) {
        if(isDead) return;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVFX(AudioManager.Instance.hurtSound);
        }
        curHealth -= damage;
        if (healthSlider != null) healthSlider.value = curHealth;
        if (curHealth <= 0) PlayerDie();
    }
   
    public void Heal(float amount) {
        if(isDead) return;
        curHealth = Mathf.Min(curHealth + amount, maxHealth);
        if (healthSlider != null) healthSlider.value = curHealth;
    }

    public void GainXP(float xp) { curExp += xp; }

    void LevelUp() {
        curLvl++;
        curExp -= expToNextLvl;
        expToNextLvl *= 1.2f;

        if (levelText != null) levelText.text = "LV. " + curLvl.ToString();
        
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OpenUpgradeMenu();
        }
    }
    
    void PlayerDie() {
        isDead = true;
        anim.SetTrigger("isDead");
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        Destroy(gameObject, 0.75f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }

    void StopDashEffect() { if (dashEffectCoroutine != null) StopCoroutine(dashEffectCoroutine); }
    void StartDashEffect() { StopDashEffect(); dashEffectCoroutine = StartCoroutine(DashEffectCoroutine()); }

    IEnumerator DashEffectCoroutine() {
        while (true) {
            GameObject glitch = Instantiate(glitchEffect, transform.position, transform.rotation);
            Destroy(glitch, 0.3f);
            yield return new WaitForSeconds(delayGlitchSeconds);
        }
    }
    
    public void ApplyPassiveBuff(PassiveBuffType type, float value)
    {
        switch (type)
        {
            case PassiveBuffType.CooldownReduction:
                passiveCooldownReduction -= value; // Ví dụ: cộng dồn thêm 10% giảm hồi chiêu
                Debug.Log($"[Passive UI] Hệ số hồi chiêu giảm xuống còn: {passiveCooldownReduction}");
                break;

            case PassiveBuffType.MovementSpeed:
                passiveMoveSpeedMultiplier += value; // Ví dụ: +0.15 tức là chạy nhanh hơn 15%
                // Ở đây ông nhân cái biến passiveMoveSpeedMultiplier này vào tốc độ di chuyển gốc của ông là xong!
                break;

            case PassiveBuffType.MaxHealth:
                passiveMaxHealthMultiplier += value;
                
                float growthThisLevel = 1f + value; 
                maxHealth *= growthThisLevel;
                curHealth *= growthThisLevel;
                 if (healthSlider != null)
                 {
                     healthSlider.maxValue = maxHealth; // Cập nhật giới hạn mới (Ví dụ: Từ 100 lên 120)
                     healthSlider.value = curHealth;   // Đẩy thanh máu hiện tại lên theo
                 }
                break;

            case PassiveBuffType.DamageBoost:
                passiveDamageMultiplier += value; // Tăng sát thương tổng lực cho cả súng lẫn kiếm
                break;
        }
    }
    
    public void OnDashButtonPressed()
    {
        if (_dashTime <= 0 && !isDashing && !isDead) 
        {
            _dashTime = dashTime; 
            isDashing = true; 
            StartDashEffect();
            
            if (dashBtnAnim != null)
            {
                dashBtnAnim.SetTrigger("Click");
            }
        }
    }
    
    public void OnBurnButtonPressed()
    {
        if (isDead) return;
        
        BurnSkill burnSkill = GetComponent<BurnSkill>() ?? GetComponentInChildren<BurnSkill>();

        if (burnSkill != null)
        {
            burnSkill.toggleBurn();
        
            if (burnBtnAnim != null)
            {
                burnBtnAnim.SetTrigger("Click");
            }
        }
    }
}