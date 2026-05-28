using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    // LỆNH NÚT: Chọn Màn 1
    public void SelectStage1()
    {
        PlayerPrefs.SetInt("CURRENT_STAGE_INDEX", 1); 
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene"); 
    }

    // LỆNH NÚT: Chọn Màn 2
    public void SelectStage2()
    {
        PlayerPrefs.SetInt("CURRENT_STAGE_INDEX", 2); 
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
}