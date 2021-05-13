using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject fileNameSetter;

    public GameObject _customized;
    public GameObject _new;
    public GameObject _open;
    
    public void SelectEasyGame()
    {
        DifficultySetter.SetEasyMode();
        //Settings.PlayMode = 0;

        SceneManager.LoadScene("LoadingScene");
    }

    public void SelectMediumGame()
    {

        DifficultySetter.SetMediumMode();
        //Settings.PlayMode = 0;

        SceneManager.LoadScene("LoadingScene");
    }

    public void SelectHardGame()
    {

        DifficultySetter.SetHardMode();
        //Settings.PlayMode = 0;

        SceneManager.LoadScene("LoadingScene");
    }

    public void SelectCustomizedGame()
    {
        _customized.SetActive(false);
        _new.SetActive(true);
        _open.SetActive(true);
    }
    public void NewGame()
    {
        Settings.PlayMode = 1;
        SceneManager.LoadScene("LoadingScene");
    }
    public void OpenGame()
    {
        Settings.PlayMode = 2;
        fileNameSetter.SetActive(true);
        //SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
