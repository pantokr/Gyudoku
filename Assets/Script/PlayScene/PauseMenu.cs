using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Displaying;
    private void Awake()
    {
        Time.timeScale = 0;
        Displaying = true;
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
        Displaying = false;
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
