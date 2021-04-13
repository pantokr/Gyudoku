using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberButtons : MonoBehaviour
{
    public GameObject MainPanel;

    private Button[] nButtons;
    private Playing playing;

    private void Start()
    {
        //nButtons = new Button[9];
        //playing = new Playing();
    }

    public void LoadNumberButtons()
    {
        Debug.Log(gameObject.name);
        for (int index = 0; index < 9; index++)
        {
            string tString = $"button_{index + 1}";
            nButtons[index] = gameObject.transform.Find(tString).GetComponent<Button>();

            int tIndex = index + 1;
            nButtons[index].onClick.AddListener(delegate { playing.HighlightCells(tIndex); });
        }
    }
}
