using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Level
{

    [System.Serializable]
    public class WaveInfo
    {
        public string waveName = "Đợt quái";
        public GameObject enemyPrefab;
        public float spawnInterval = 1f;
        public int maxEnemiesInWave = 20;
        public float startAtSecond = 0f;

    }
    
    [CreateAssetMenu(fileName = "NewLevelCard", menuName = "Level System/Level Card")]
    public class LevelData : ScriptableObject
    {
        public string levelName = "Màn 1";
        public float levelDuration = 60f;
        public List<WaveInfo> wavesPool; 
        public GameObject bossPrefab;
    }
}