using UnityEngine;

    public class DeathAfterTime : MonoBehaviour
    {
        public float deathTime = 0.75f;
        void Start()
        {
            Destroy(gameObject, deathTime);
        }
    }
