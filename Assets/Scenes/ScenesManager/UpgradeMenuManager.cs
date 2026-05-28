using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class UpgradeMenuManager : MonoBehaviour
{
    [Header("=== UI HIỂN THỊ SỐ SAO CÒN LẠI ===")]
    public TMP_Text totalStarsText; // Kéo Text hiển thị số Sao còn lại vào đây

    [Header("=== TEXT LEVEL HIỆN TẠI ===")]
    public TMP_Text hpLevelText;
    public TMP_Text atkLevelText;
    public TMP_Text cdrLevelText;
    public TMP_Text speedLevelText;

    [Header("=== TEXT GIÁ TIỀN (SỐ SAO CẦN CHI) ===")]
    public TMP_Text hpCostText;
    public TMP_Text atkCostText;
    public TMP_Text cdrCostText;
    public TMP_Text speedCostText;
    
    [Header("=== NÚT BẤM (ĐỂ KHÓA KHI MAX) ===")]
    public Button hpButton;
    public Button atkButton;
    public Button cdrButton;
    public Button speedButton;
    
    [Header("=== Animator ===")]
    public Animator hpAnimator;
    public Animator atkAnimator;
    public Animator cdrAnimator;
    public Animator speedAnimator;
    public Animator refundAnimator;
    
    [Header("=== NÚT REFUND (ĐỂ ẨN/HIỆN KHI CẦN) ===")]
    public Button refundButton;
    
    [Header("=== CẤU HÌNH GIỚI HẠN CẤP TỐI ĐA ===")]
    public int maxLevel = 5;

    private int _totalStarsAvailable; // Số Sao thực tế còn lại để mua đồ
    private int _hpLevel;
    private int _atkLevel;
    private int _cdLevel;
    private int _speedLevel;

    void OnEnable()
    {
        // Mỗi lần bảng nâng cấp bật lên, tự động cập nhật dữ liệu mới nhất
        RefreshUpgradeMenu();
    }

    public void RefreshUpgradeMenu()
    {
        // 1. ĐỌC DỮ LIỆU SAO THẮNG ĐƯỢC TỪ CÁC MÀN CHƠI (Mặc định bằng 0 nếu chưa chơi)
        int starStage1 = PlayerPrefs.GetInt("STAGE_1_STARS", 0); 
        int starStage2 = PlayerPrefs.GetInt("STAGE_2_STARS", 0);
        int starStage3 = PlayerPrefs.GetInt("STAGE_3_STARS", 0);
        int totalStarsEarned = starStage1 + starStage2 + starStage3;

        int totalStarsSpent = PlayerPrefs.GetInt("STARS_SPENT", 0);

        _totalStarsAvailable = totalStarsEarned - totalStarsSpent;
        
        _hpLevel = PlayerPrefs.GetInt("STAT_HP_LEVEL", 1);
        _atkLevel = PlayerPrefs.GetInt("STAT_ATK_LEVEL", 1);
        _cdLevel = PlayerPrefs.GetInt("STAT_CDR_LEVEL", 1);
        _speedLevel = PlayerPrefs.GetInt("STAT_SPEED_LEVEL", 1);
        
        if (totalStarsText != null) totalStarsText.text = "STARS: " + _totalStarsAvailable.ToString();
        
        if(refundButton != null)  refundButton.interactable = (totalStarsSpent > 0);
        
        UpdateRowUI(hpLevelText, hpCostText, hpButton, _hpLevel);
        UpdateRowUI(atkLevelText, atkCostText, atkButton, _atkLevel);
        UpdateRowUI(cdrLevelText, cdrCostText, cdrButton, _cdLevel);
        UpdateRowUI(speedLevelText, speedCostText, speedButton, _speedLevel);
    }

    private void UpdateRowUI(TMP_Text lvlText, TMP_Text costText, Button button, int curLvl)
    {
        if (lvlText != null) lvlText.text = "LV. " + curLvl;

        if (curLvl >= maxLevel)
        {
            if (costText != null) costText.text = "MAX";
            if (button != null) button.interactable = false;
        }
        else
        {
            if (costText != null) costText.text = curLvl + " STAR";
            if(button != null) button.interactable = true;
        }
    }
    
    public void UpgradeHP()
    {
        if(hpAnimator != null) hpAnimator.SetTrigger("Click");
        if (_hpLevel >= maxLevel) return;
        int cost = _hpLevel; // Giá bằng đúng level hiện tại

        if (_totalStarsAvailable >= cost)
        {
            BuyUpgrade("STAT_HP_LEVEL", cost);
            Debug.Log("💸 Đã nâng cấp HP vĩnh viễn lên cấp: " + _hpLevel);
        }
        else
        {
            Debug.Log("❌ Không đủ Sao để nâng cấp HP!");
        }
    }
    
    public void UpgradeATK()
    {
        if(atkAnimator != null) atkAnimator.SetTrigger("Click");
        if (_atkLevel >= maxLevel) return;
        int cost = _atkLevel;

        if (_totalStarsAvailable >= cost)
        {
            BuyUpgrade("STAT_ATK_LEVEL", cost);
            Debug.Log("💸 Đã nâng cấp ATK vĩnh viễn lên cấp: " + _atkLevel);
        }
        else
        {
            Debug.Log("❌ Không đủ Sao để nâng cấp ATK!");
        }
    }

    public void UpgradeCDR()
    {
        if(cdrAnimator != null)  cdrAnimator.SetTrigger("Click");
        if (_cdLevel >= maxLevel) return;
        int cost = _cdLevel;

        if (_totalStarsAvailable >= cost)
        {
            BuyUpgrade("STAT_CDR_LEVEL", cost);
            Debug.Log("💸 Đã nâng cấp CDR vĩnh viễn lên cấp: " + _cdLevel);
        }
        else
        {
            Debug.Log("❌ Không đủ Sao để nâng cấp Cooldown Reduction!");
        }
    }

    public void UpgradeSpeed()
    {
        if(speedAnimator != null) speedAnimator.SetTrigger("Click");
        if(_speedLevel >= maxLevel) return;
        int cost = _speedLevel;

        if (_totalStarsAvailable >= cost)
        {
            BuyUpgrade("STAT_SPEED_LEVEL", cost);
            Debug.Log("💸 Đã nâng cấp Move Speed vĩnh viễn lên cấp: " + _speedLevel);
        }
        else
        {
            Debug.Log("❌ Không đủ Sao để nâng cấp Move Speed!");
        }
    }

    private void BuyUpgrade(string statLvl, int cost)
    {
        int totalStarsSpent = PlayerPrefs.GetInt("STARS_SPENT", 0);
        totalStarsSpent += cost;
        PlayerPrefs.SetInt("STARS_SPENT", totalStarsSpent);

        int curLvl = PlayerPrefs.GetInt(statLvl, 1);
        PlayerPrefs.SetInt(statLvl, curLvl + 1);
        PlayerPrefs.Save();
        
        RefreshUpgradeMenu();
    }

    public void RefundAllStars()
    {
        if(refundAnimator != null) refundAnimator.SetTrigger("Click");
        int totalStarsSpent = PlayerPrefs.GetInt("STARS_SPENT", 0);

        if (totalStarsSpent <= 0) return;
        
        PlayerPrefs.SetInt("STARS_SPENT", 0);
        PlayerPrefs.SetInt("STAT_HP_LEVEL", 1);
        PlayerPrefs.SetInt("STAT_ATK_LEVEL", 1);
        PlayerPrefs.SetInt("STAT_CDR_LEVEL", 1);
        PlayerPrefs.SetInt("STAT_SPEED_LEVEL", 1);
        
        PlayerPrefs.Save();
        RefreshUpgradeMenu();
    }
}