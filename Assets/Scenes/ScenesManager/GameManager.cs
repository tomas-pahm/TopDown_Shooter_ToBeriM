using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
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
    
    [Header("=== UI PANEL THẤT BẠI (GAMEOVER) ===")]
    public GameObject gameOverPanel;    
    public TMP_Text survivalTimeText;
    
    [Header("Animator")]
    public Animator returnToMMAnimator;
    public Animator playAgainAnimator;
    public Animator victoryReturnToMMAnimator;
    
    
    // 🔥 SỬA BIẾN NÀY TỪ GameObject[] SANG Image[] ĐỂ ĐỒNG BỘ ĐỔI MÀU SAO NHE ÔNG!
    public Image[] starImages;      

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
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
        if (returnToMMAnimator != null) returnToMMAnimator.SetTrigger("Click");
        if (victoryReturnToMMAnimator != null) victoryReturnToMMAnimator.SetTrigger("Click");
        StartCoroutine(DelayReturnToMainMenu());
    }
    
    public void TriggerVictory()
    {
        if (_isGameEnded) return;
        _isGameEnded = true;
        
        Time.timeScale = 0f;
        
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

    public void PlayAgain()
    {
        if (playAgainAnimator != null) playAgainAnimator.SetTrigger("Click");
        StartCoroutine(DelayPlayAgain());
    }

    public void TriggerGameOver()
    {
        if (_isGameEnded) return;
        _isGameEnded = true;
        
        Time.timeScale = 0f;
        
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        
        int survivalTimeSeconds = Mathf.RoundToInt(_matchTimer);
        int minutes = survivalTimeSeconds / 60;
        int seconds = survivalTimeSeconds % 60;

        if (survivalTimeText != null)
        {
            survivalTimeText.text = string.Format("SURVIVED: {0:00}:{1:00}", minutes, seconds);
        }
    }

    IEnumerator DelayPlayAgain()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1f;
        string curSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(curSceneName);
    }

    IEnumerator DelayReturnToMainMenu()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}