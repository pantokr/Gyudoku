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
        SceneManager.LoadScene("LoadingScreen");
    }

    public void MediumGame()
    {
        Settings.EmptyC1 = Random.Range(5, 7);
        Settings.EmptyC2 = 11 - Settings.EmptyC1;
        Settings.EmptyMiddle = Random.Range(2, 6);
        Settings.PatternCode = Random.Range(0, 2);
        SceneManager.LoadScene("LoadingScreen");
    }

    public void HardGame()
    {
        Settings.EmptyC1 = Random.Range(5, 7);
        Settings.EmptyC2 = 11 - Settings.EmptyC1;
        Settings.EmptyMiddle = 5;
        Settings.PatternCode = Random.Range(0, 2);
        SceneManager.LoadScene("LoadingScreen");
    }

    public void CustomizedGame()
    {
        Settings.customizedMode = true;
        panel.transform.Find("New").gameObject.SetActive(true);
        panel.transform.Find("Open").gameObject.SetActive(true);
    }
    public void NewGame()
    {
        Settings.customizedMode = true;
        panel.transform.Find("New").gameObject.SetActive(true);
        panel.transform.Find("Open").gameObject.SetActive(true);
    }
    public void OpenGame()
    {
        Settings.customizedMode = true;
        panel.transform.Find("New").gameObject.SetActive(true);
        panel.transform.Find("Open").gameObject.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
