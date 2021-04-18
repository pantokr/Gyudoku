using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Image bar;
    void Start()
    {
        StartCoroutine(LoadYourAsyncScene());
    }
    
    IEnumerator  LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PlayScene");
        
        while (asyncLoad.progress < 1)
        {
            bar.fillAmount = asyncLoad.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}
