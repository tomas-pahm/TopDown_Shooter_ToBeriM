using TMPro;
using UnityEngine;

namespace DefaultNamespace.Level
{
    public class LevelUIManager : MonoBehaviour
    {
        public TextMeshProUGUI timerText;
        private EnemySpawner spawner;

        void Start()
        {
            spawner = FindFirstObjectByType<EnemySpawner>();
        }

        void Update()
        {
            if (spawner != null && timerText != null)
            {
                float timeRemaining = spawner.GetRemainingTime();
                
                float minutes = Mathf.FloorToInt(timeRemaining / 60);
                float seconds = Mathf.FloorToInt(timeRemaining % 60);
                
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }
}