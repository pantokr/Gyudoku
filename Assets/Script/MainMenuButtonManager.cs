using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonManager : MonoBehaviour
{
    public Sprite easy;
    public Sprite medium;
    public Sprite hard;

    public void EasyGame()
    {
        Settings.Background = easy;
        Settings.EmptyC1 = Random.Range(4, 7);
        Settings.EmptyC2 = 10 - Settings.EmptyC1;
        Settings.EmptyMiddle = Random.Range(2, 4);
        Settings.PatternCode = 0;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void MediumGame()
    {
        Settings.Background = medium;
        Settings.Background = easy;
        Settings.EmptyC1 = Random.Range(5, 7);
        Settings.EmptyC2 = 11 - Settings.EmptyC1;
        Settings.EmptyMiddle = Random.Range(2, 6);
        Settings.PatternCode = Random.Range(0, 2);
        SceneManager.LoadScene("LoadingScreen");
    }

    public void HardGame()
    {
        Settings.Background = hard;
        Settings.EmptyC1 = Random.Range(5, 7);
        Settings.EmptyC2 = 11 - Settings.EmptyC1;
        Settings.EmptyMiddle = 5;
        Settings.PatternCode = Random.Range(0, 2);
        SceneManager.LoadScene("LoadingScreen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
