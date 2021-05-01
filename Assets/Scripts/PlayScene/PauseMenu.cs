using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Displaying;
    private void Awake()
    {
        transform.Find("VolumeSlider").GetComponent<Slider>().value = Settings.Volume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || (Input.GetKeyDown(KeyCode.Escape) && gameObject.activeSelf))
        {
            gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(true);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
