using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu_C : MonoBehaviour
{
    public static bool isDisplayed;
    private void Awake()
    {
        Time.timeScale = 0;
        isDisplayed = true;
        transform.Find("VolumeSlider").GetComponent<Slider>().value = Settings.Volume;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isDisplayed = false;
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
