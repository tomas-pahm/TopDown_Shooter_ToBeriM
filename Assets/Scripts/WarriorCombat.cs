using UnityEngine;
using UnityEngine.UI;

public class WarriorCombat : MonoBehaviour
{
    [Header("UI Thanh Nộ")]
    public Slider wrathfulnessBar;
    public Slider superWrathfulnessBar;
    
    [Header("Cấu hình Vùng Chém")]
    public Transform attackPoint;   
    public float attackRange; 
    public LayerMask playerLayer;    
    public int attackDamage = 15;  
    
    [Header("Cơ chế Phẫn Nộ (Wrathful)")]
    public GameObject fireEffectChild;
    public GameObject IfritJambeEffectChild;
    [Header("Cấu hình Phẫn Nộ (Super Wrathful)")]
    public float timeUntilWrathful = 10f;
    public float speedMultiplier = 1.5f; 
    public int damageMultiplier = 2; 
    public float rangeMultiplier = 1.5f;
    [Header("Cấu hình Siêu Phẫn Nộ (Super Wrathful)")]
    public float superSpeedMultiplier = 2f;   // Điên lửa xanh thì chạy nhanh gấp đôi!
    public int superDamageMultiplier = 3;

    private float originalRange;
    private float timeDoes0Damage = 0f;
    private float timeDoes0DamageWW = 0f;
    private bool isWrathActive = false;
    private bool isSuperWrathActive = false;
    private bool isBuffApplied = false;
    private BossWarrior bossWarrior; 
    private float originalMoveSpeed;

    void Start()
    {
        bossWarrior = GetComponent<BossWarrior>();
        timeDoes0Damage = 0f;
        timeDoes0DamageWW = 0f;
        
        originalRange = attackRange;
        if(bossWarrior  != null) originalMoveSpeed = bossWarrior.moveSpeed;
        
        if(fireEffectChild != null) fireEffectChild.SetActive(false);
        if(IfritJambeEffectChild != null) IfritJambeEffectChild.SetActive(false);

        if (wrathfulnessBar != null)
        {
            wrathfulnessBar.maxValue = timeUntilWrathful;
            wrathfulnessBar.value = timeDoes0Damage;
        }

        if (superWrathfulnessBar != null)
        {
            superWrathfulnessBar.maxValue = timeUntilWrathful;
            superWrathfulnessBar.value = timeDoes0DamageWW;
        }
    }
    
    void Update()
    {
        if (!isWrathActive)
        {
            timeDoes0Damage += Time.deltaTime;
            Wrathful();
        }

        if (isWrathActive && isBuffApplied)
        {
            timeDoes0DamageWW += Time.deltaTime;
            SuperWrathful();
        }
        
        if(wrathfulnessBar != null)
        {
            wrathfulnessBar.value = timeDoes0Damage;
        }

        if (superWrathfulnessBar != null)
        {
            superWrathfulnessBar.value = timeDoes0DamageWW;
        }
    }
    
    public void HitTarget()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            PlayerController playerController = hitPlayer.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                int finalDamage = attackDamage;
                if (isSuperWrathActive) finalDamage = attackDamage * superDamageMultiplier;
                else if (isWrathActive) finalDamage = attackDamage * damageMultiplier;
                playerController.TakeDamage(finalDamage);
                CoolDownWrath();
                Debug.Log("🛡️ Boss chém trúng Player! Trừ " + finalDamage + " máu.");
            }
        }
        else
        {
            Debug.Log("💨 Boss chém hụt mục tiêu!");
        }
    }

    public void Wrathful()
    {
        if (timeDoes0Damage >= timeUntilWrathful && !isWrathActive)
        {
            Debug.Log("Cay cú 🔥");
            isWrathActive = true;
            if (bossWarrior != null)
            {
                bossWarrior.StartGuardState();
                Invoke("EndGuardAndBuff", 2f);
            }
        }
    }

    public void SuperWrathful()
    {
        if (timeDoes0DamageWW >= timeUntilWrathful && !isSuperWrathActive)
        {
            Debug.Log("Siêu cay cú 🔥🔥 - IFRIT JAMBE!");
            isSuperWrathActive = true;
            if (bossWarrior != null)
            {
                bossWarrior.StartGuardState();
                Invoke("EndGuardAndBuff", 2f);
            }
        }
    }

    public void EndGuardAndBuff()
    {
        CancelInvoke("EndGuardAndBuff");
        
        if (isSuperWrathActive)
        {
            if(fireEffectChild != null)
                fireEffectChild.SetActive(false);
            if(IfritJambeEffectChild != null)
                IfritJambeEffectChild.SetActive(true);
            if (bossWarrior != null)
            {
                bossWarrior.moveSpeed = originalMoveSpeed * superSpeedMultiplier;
            }
            isBuffApplied = true;
            attackRange = originalRange * rangeMultiplier;
        }
        
        else if (isWrathActive)
        {
            if(fireEffectChild !=null)
                fireEffectChild.SetActive(true);
            if (bossWarrior != null)
            {
                bossWarrior.moveSpeed = originalMoveSpeed * speedMultiplier;
            }
            isBuffApplied = true;
            attackRange = originalRange * rangeMultiplier;
        }

        

        if (bossWarrior != null)
        {
            bossWarrior.FinishedAttackInsideAnimation();
        }
    }

    public void CoolDownWrath()
    {
        if(!isWrathActive && !isSuperWrathActive) return;
        Debug.Log("Hết Cay Cú");
        isWrathActive = false;
        isSuperWrathActive = false;
        isBuffApplied = false;
        timeDoes0DamageWW = 0f;
        timeDoes0Damage = 0f;

        if (fireEffectChild != null)
        {
            fireEffectChild.SetActive(false);
        }

        if (IfritJambeEffectChild != null)
        {
            IfritJambeEffectChild.SetActive(false);
        }

        attackRange = originalRange;

        if (bossWarrior != null)
        {
            bossWarrior.moveSpeed = originalMoveSpeed;
        }
    }
    
    public void EndAttack()
    {
        if (bossWarrior != null)
        {
            bossWarrior.FinishedAttackInsideAnimation(); 
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}