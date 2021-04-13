using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberButtons : MonoBehaviour
{
    public GameObject MainPanel;

    private Button[] numberButtons;
    private Playing playing;

    private void Start()
    {
        numberButtons = new Button[9];
        playing = new Playing();
    }

    public void LoadNumberButtons()
    {
        GameObject nbObject = MainPanel.transform.Find("NumberButtons").gameObject;


        for (int index = 0; index < 9; index++)
        {
            string tString = $"button_{index + 1}";
            numberButtons[index] = nbObject.transform.Find(tString).GetComponent<Button>();

            int tIndex = index + 1;
            numberButtons[index].onClick.AddListener(delegate { playing.HighlightCells(tIndex); });
        }
    }
}
