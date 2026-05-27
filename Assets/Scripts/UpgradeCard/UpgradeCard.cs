using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCard : MonoBehaviour
{
    [Header("UI Components")]
    public Image weaponIcon;          
    public TextMeshProUGUI cardText; 
    
    [Header("Stars System (Xếp theo thứ tự từ 1 đến 3)")]
    public GameObject[] yellowStars;  
    public GameObject redStar;        

    private System.Action onCardSelectedAction; 
    
    public void SetupCard(Sprite icon, string description, int level, System.Action onSelectedCallback)
    {
        if (weaponIcon != null) weaponIcon.sprite = icon;
        if (cardText != null) cardText.text = description;
        
        onCardSelectedAction = onSelectedCallback;
        
        if (level == 5)
        {
            foreach (var star in yellowStars)
            {
                if (star != null) star.SetActive(false);
            }
            if (redStar != null) redStar.SetActive(true);
        }
        else
        {
            if (redStar != null) redStar.SetActive(false);
            
            for (int i = 0; i < yellowStars.Length; i++)
            {
                if (yellowStars[i] != null)
                {
                    yellowStars[i].SetActive(i < level);
                }
            }
        }
    }
    
    public void OnCardClicked()
    {
        onCardSelectedAction?.Invoke();
    }
}