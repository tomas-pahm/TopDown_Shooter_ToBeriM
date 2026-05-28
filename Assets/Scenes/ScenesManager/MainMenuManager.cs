using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Animator của Button")] 
    public Animator playAnimBtn;
    public Animator upgradeAnimBtn;
    public Animator exitAnimBtn;
    
    [Header("Tên Scene Trận Đấu Dùng Chung")]
    public string gameplaySceneName = "SampleScene"; 

    [Header("=== UI PANELS ===")]
    public GameObject mainMenuPanel;
    public GameObject upgradePanel;
    public GameObject stageSelectPanel;
    
    [Header("=== HỆ THỐNG SAO TRÊN CARD MÀN 1 ===")]
    public Image[] stage1Stars;
    public Image[] stage2Stars;
    public Image[] stage3Stars;

    void Start()
    {
        // Vừa vào Menu thì hiện bảng chính, ẩn các bảng khác đi
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (stageSelectPanel != null) stageSelectPanel.SetActive(false);
    }
    
    public void RefreshStageStarsUI()
    {
        // ==========================================
        // 🗂️ 1. ĐỌC DỮ LIỆU SAO CỦA CẢ 3 MÀN TỪ MÁY RA
        // ==========================================
        int s1 = PlayerPrefs.GetInt("STAGE_1_STARS", 0);
        int s2 = PlayerPrefs.GetInt("STAGE_2_STARS", 0);
        int s3 = PlayerPrefs.GetInt("STAGE_3_STARS", 0);
    
        // ==========================================
        // ⭐ MÀN 1: QUÉT VÀ THẮP SÁNG SAO
        // ==========================================
        for (int i = 0; i < stage1Stars.Length; i++)
        {
            if (stage1Stars[i] != null)
            {
                if (i < s1)
                {
                    stage1Stars[i].color = Color.yellow; // Đạt -> Đổi thành Sao Vàng
                }
                else
                {
                    stage1Stars[i].color = new Color(0.2f, 0.2f, 0.2f, 1f); // Chưa đạt -> Sao Đen
                }
            }
        }

        // ==========================================
        // ⭐ MÀN 2: QUÉT VÀ THẮP SÁNG SAO
        // ==========================================
        for (int i = 0; i < stage2Stars.Length; i++)
        {
            if (stage2Stars[i] != null)
            {
                if (i < s2)
                {
                    stage2Stars[i].color = Color.yellow;
                }
                else
                {
                    stage2Stars[i].color = new Color(0.2f, 0.2f, 0.2f, 1f);
                }
            }
        }

        // ==========================================
        // ⭐ MÀN 3: QUÉT VÀ THẮP SÁNG SAO
        // ==========================================
        for (int i = 0; i < stage3Stars.Length; i++)
        {
            if (stage3Stars[i] != null)
            {
                if (i < s3)
                {
                    stage3Stars[i].color = Color.yellow;
                }
                else
                {
                    stage3Stars[i].color = new Color(0.2f, 0.2f, 0.2f, 1f);
                }
            }
        }
    }


    public void PlayGame()
    {
        if (playAnimBtn != null) playAnimBtn.SetTrigger("Click");
        StartCoroutine(DelayPlayGame());
    }

    IEnumerator DelayPlayGame()
    {
        yield return new WaitForSeconds(0.15f);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (stageSelectPanel != null) stageSelectPanel.SetActive(true);

        
        RefreshStageStarsUI(); 
    }
    public void BackFromStageSelect()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (stageSelectPanel != null) stageSelectPanel.SetActive(false);
    }

    // ⚔️ Nút chọn MÀN 1
    public void SelectStage1()
    {
        PlayerPrefs.SetInt("CURRENT_STAGE_INDEX", 1); // Ghi ngầm xuống máy là chọn Màn 1
        PlayerPrefs.Save();
        StartCoroutine(DelayLoadScene()); // Chạy trình hoãn để load vào trận
    }

    // ⚔️ Nút chọn MÀN 2
    public void SelectStage2()
    {
        PlayerPrefs.SetInt("CURRENT_STAGE_INDEX", 2); // Ghi ngầm xuống máy là chọn Màn 2
        PlayerPrefs.Save();
        StartCoroutine(DelayLoadScene());
    }

    // ⚔️ Nút chọn MÀN 3
    public void SelectStage3()
    {
        PlayerPrefs.SetInt("CURRENT_STAGE_INDEX", 3); // Ghi ngầm xuống máy là chọn Màn 3
        PlayerPrefs.Save();
        StartCoroutine(DelayLoadScene());
    }

    // Trình hoãn thời gian để kịp diễn xong hiệu ứng nút bấm trước khi chuyển Scene
    IEnumerator DelayLoadScene()
    {
        yield return new WaitForSeconds(0.15f); 
        SceneManager.LoadScene(gameplaySceneName); 
    }

    // 🌟 LỆNH NÚT MỞ BẢNG NÂNG CẤP VĨNH VIỄN (Ở Menu chính)
    public void OpenUpgradeMenu()
    {
        if (upgradeAnimBtn != null) upgradeAnimBtn.SetTrigger("Click");
        StartCoroutine(DelayUpgradeMenu());
    }

    IEnumerator DelayUpgradeMenu()
    {
        yield return new WaitForSeconds(0.15f);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(true);
    }

    // ↩️ LỆNH NÚT QUAY LẠI CỦA BẢNG NÂNG CẤP
    public void BackToMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }
    
    public void ExitGame()
    {
        if (exitAnimBtn != null) exitAnimBtn.SetTrigger("Click");
        Debug.Log("🔌 Đã bấm nút thoát game!");
        StartCoroutine(DelayExitGame()); 
    }

    IEnumerator DelayExitGame()
    {
        yield return new WaitForSeconds(0.15f);
        Application.Quit();
    }
}