using UnityEngine;
using UnityEngine.UI; // BẮT BUỘC PHẢI CÓ DÒNG NÀY ĐỂ ĐIỀU KHIỂN IMAGE CHẤN ƠI
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("=== CẤU HÌNH MÀN CHƠI PHÁT CHIẾN ===")]
    public string currentStageKey = "STAGE_1_STARS"; 

    [Header("=== QUẢN LÝ BỘ ĐẾM THỜI GIAN ===")]
    private float _matchTimer = 0f; 
    private bool _isGameEnded = false;

    [Header("=== UI PANEL KẾT THÚC TRẬN ===")]
    public GameObject victoryPanel;      
    public TMP_Text clearTimeText;       
    
    // 🔥 SỬA BIẾN NÀY TỪ GameObject[] SANG Image[] ĐỂ ĐỒNG BỘ ĐỔI MÀU SAO NHE ÔNG!
    public Image[] starImages;      

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!_isGameEnded)
        {
            _matchTimer += Time.deltaTime;
        }
    }
    
    public void ReturnToMainMenu()
    {
        // Điền đúng tên Scene Menu chính của ông vào đây (Ví dụ tên là "MainMenuScene")
        SceneManager.LoadScene("MainMenu"); 
    }
    
    public void TriggerVictory()
    {
        if (_isGameEnded) return;
        _isGameEnded = true;
        
        int clearTimeSeconds = Mathf.RoundToInt(_matchTimer);
        
        int starsAwarded = 1; 

        if (clearTimeSeconds <= 120) 
        {
            starsAwarded = 3;
        }
        else if (clearTimeSeconds <= 150) 
        {
            starsAwarded = 2;
        }
        
        int oldStars = PlayerPrefs.GetInt(currentStageKey, 0);
        if (starsAwarded > oldStars)
        {
            PlayerPrefs.SetInt(currentStageKey, starsAwarded);
            PlayerPrefs.Save(); 
            Debug.Log($"💾 Đã lưu kỷ lục mới: đạt {starsAwarded} Sao cho {currentStageKey}!");
        }
        
        if (victoryPanel != null) victoryPanel.SetActive(true);

        int minutes = clearTimeSeconds / 60;
        int seconds = clearTimeSeconds % 60;
        if (clearTimeText != null) clearTimeText.text = string.Format("TIME: {0:00}:{1:00}", minutes, seconds);
        
        // ====================================================================
        // 🔥 ĐOẠN ĐỒNG BỘ ĐỔI MÀU SAO: BIẾN SAO ĐEN THÀNH VÀNG TRÊN PANEL VICTORY
        // ====================================================================
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                if (i < starsAwarded)
                {
                    // Được Sao nào thì thắp sáng Sao đó thành MÀU VÀNG rực rỡ!
                    starImages[i].color = Color.yellow; 
                }
                else
                {
                    // Sao nào chưa đạt được thì ép nó thành MÀU ĐEN/XÁM
                    starImages[i].color = new Color(0.2f, 0.2f, 0.2f, 1f); 
                }
            }
        }
    }
}