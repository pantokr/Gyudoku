using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonManager : MonoBehaviour
{
    public GameObject panel;

    public void EasyGame()
    {
        Settings.EmptyC1 = Random.Range(4, 7);
        Settings.EmptyC2 = 10 - Settings.EmptyC1;
        Settings.EmptyMiddle = Random.Range(2, 4);
        Settings.PatternCode = 0;
        Settings.PlayMode = 0;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void MediumGame()
    {
        Settings.EmptyC1 = Random.Range(5, 7);
        Settings.EmptyC2 = 11 - Settings.EmptyC1;
        Settings.EmptyMiddle = Random.Range(2, 6);
        Settings.PatternCode = Random.Range(0, 2);
        Settings.PlayMode = 0;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void HardGame()
    {
        Settings.EmptyC1 = Random.Range(5, 7);
        Settings.EmptyC2 = 11 - Settings.EmptyC1;
        Settings.EmptyMiddle = 5;
        Settings.PatternCode = Random.Range(0, 2);
        Settings.PlayMode = 0;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void CustomizedGame()
    {
        panel.transform.Find("Customized").gameObject.SetActive(false);
        panel.transform.Find("New").gameObject.SetActive(true);
        panel.transform.Find("Open").gameObject.SetActive(true);
    }
    public void NewGame()
    {
        Settings.PlayMode = 1;
        SceneManager.LoadScene("LoadingScreen");
    }
    public void OpenGame()
    {
        Settings.PlayMode = 2;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
