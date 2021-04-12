using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Sprite easy;
    public Sprite medium;
    public Sprite hard;

    public void EasyGame()
    {
        Settings.Background = easy;
        Settings.MissingNumberCnt = Random.Range(9, 13);
        SceneManager.LoadScene("LoadingScreen");
    }
    
    public void MediumGame()
    {
        Settings.Background = medium;
        Settings.MissingNumberCnt = Random.Range(18, 22);
        SceneManager.LoadScene("LoadingScreen");
    }
    
    public void HardGame()
    {
        Settings.Background = hard;
        Settings.MissingNumberCnt = Random.Range(40, 50);
        SceneManager.LoadScene("LoadingScreen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
