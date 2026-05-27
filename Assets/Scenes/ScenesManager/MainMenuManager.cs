using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // BẮT BUỘC PHẢI CÓ DÒNG NÀY ĐỂ CHUYỂN SCENE

public class MainMenuManager : MonoBehaviour
{
    [Header("Animator của Button")] 
    public Animator playAnimBtn;
    public Animator upgradeAnimBtn;
    public Animator exitAnimBtn;
    
    [Header("Tên Scene Trận Đấu")]
    public string gameplaySceneName = "SampleScene"; // Điền đúng tên Scene đánh quái của ông vào đây

    [Header("UI Panels (Nếu có)")]
    public GameObject mainMenuPanel;
    public GameObject upgradePanel;

    void Start()
    {
        // Vừa vào Menu thì hiện bảng chính, ẩn bảng nâng cấp đi
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }

    // 🚀 LỆNH NÚT START GAME: Click phát load vào map vả dơi liền
    public void PlayGame()
    {
        Time.timeScale = 1f; // Đảm bảo thời gian game chạy bình thường
        playAnimBtn.SetTrigger("Click");
        StartCoroutine(DelayLoadScene()); 
    }

    // Trình hoãn thời gian để nút kịp co bóp
    IEnumerator DelayLoadScene()
    {
        yield return new WaitForSeconds(0.15f); // Đợi 0.15 giây cho Animation chạy xong
        SceneManager.LoadScene(gameplaySceneName); // Diễn xong rồi mới bay vào vả dơi!
    }

    // 🌟 LỆNH NÚT MỞ BẢNG NÂNG CẤP VĨNH VIỄN
    public void OpenUpgradeMenu()
    {
        upgradeAnimBtn.SetTrigger("Click");
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(true);
    }

    // ↩️ LỆNH NÚT QUAY LẠI MENU CHÍNH
    public void BackToMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }
    
    public void ExitGame()
    {
        exitAnimBtn.SetTrigger("Click");
        Debug.Log("🔌 Đã bấm nút thoát game!");
        StartCoroutine(DelayExitGame()); // Lệnh này sẽ có tác dụng khi build ra file APK/PC
    }

    IEnumerator DelayExitGame()
    {
        yield return new WaitForSeconds(0.15f);
        Application.Quit();
    }
}