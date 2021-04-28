using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogHint : MonoBehaviour
{
    public static int cnt;
    public GameObject hintButton;
    public GameObject sudokuBoard;
    public GameObject mainPanel;

    private Animation pusher;
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
        //start animation
        pusher = sudokuBoard.GetComponent<Animation>();
        pusher.Play("_Pusher");
        SetVisible(false);

        textstr = (string[])texts.Clone();
        cnt = 0;

        //대화 상자 켜기
        gameObject.SetActive(true);
        hintButton.GetComponent<Button>().enabled = false;
        _dialogText = transform.Find("DialogText").GetComponent<Text>();

        ChangeText();
    }
    public void ChangeText()
    {
        if (cnt == textstr.Length)
        {
            //end animation
            pusher.Stop("_Pusher");
            sudokuBoard.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            SetVisible(true);

            gameObject.SetActive(false);
            hintButton.GetComponent<Button>().enabled = true;
            return;
        }
        //print(textstr[cnt]);
        _dialogText.text = textstr[cnt];

        cnt++;
        //isDisplayed = false;
    }

    private void SetVisible(bool onf)
    {
        mainPanel.transform.Find("NumberHighlighter").gameObject.SetActive(onf);
        mainPanel.transform.Find("ManualTools").gameObject.SetActive(onf);
        mainPanel.transform.Find("Finisher").gameObject.SetActive(onf);
        mainPanel.transform.Find("AutoTools").gameObject.SetActive(onf);
    }
}


