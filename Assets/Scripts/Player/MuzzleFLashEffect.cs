using UnityEngine;

public class MuzzleFLashEffect : MonoBehaviour
{
    public float flashTime = 0.05f;

    public void Activate()
    {
        gameObject.SetActive(true);
        Invoke("Deactivate", flashTime);
    }
    
    void Deactivate()
    {
     gameObject.SetActive(false);   
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
