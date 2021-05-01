using System;
using UnityEngine;

public class DialogScript : MonoBehaviour
{
    //public static bool isDisplayed;

    private void Awake()
    {
        //isDisplayed = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Hide();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        //isDisplayed = false;
    }
}
