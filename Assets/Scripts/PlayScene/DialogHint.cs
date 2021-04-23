using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogHint : MonoBehaviour
{
    public static int cnt;
    public GameObject hintButton;

    private string[] textstr;

    private Text _dialogText;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeText();
        }
    }

    // hint button >> hint manager >> dialog
    public void StartDialog(string[] texts)
    {
        textstr = (string[]) texts.Clone();
        cnt = 0;
        
        gameObject.SetActive(true);
        hintButton.GetComponent<Button>().enabled = false;
        _dialogText = transform.Find("DialogText").GetComponent<Text>();
        
        ChangeText();
    }
    public void ChangeText()
    {
        if (cnt == textstr.Length)
        {
            gameObject.SetActive(false);
            hintButton.GetComponent<Button>().enabled = true;
            return;
        }
        print(textstr[cnt]);
        _dialogText.text = textstr[cnt];
        
        cnt++;
        //isDisplayed = false;
    }
}
