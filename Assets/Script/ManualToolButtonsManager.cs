using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualToolButtonsManager : MonoBehaviour
{
    public static bool onMemo = false;
    public static bool onEraser = false;

    //FB92C4 251, 196, 146
    public GameObject memoButton;
    public GameObject eraserButton;

    private void Start()
    {
        Button _memo = memoButton.GetComponent<Button>();
        Button _eraser = eraserButton.GetComponent<Button>();

        _memo.onClick.AddListener(delegate { TurnMemo(); });
        _eraser.onClick.AddListener(delegate { TurnEraser(); });
    }

    public void TurnMemo()
    {
        onMemo = !onMemo;
        Image img = memoButton.transform.GetComponent<Image>();
        Color c = img.color;
        if (onMemo)
        {
            c = new Color(251 / 255f, 196 / 255f, 148 / 255f, 0.5f);
            img.color = c;
        }
        else
        {
            c = new Color(1f, 1f, 1f, 1f);
            img.color = c;
        }
    }

    public void TurnEraser()
    {
        onEraser = !onEraser;
        Image img = eraserButton.transform.GetComponent<Image>();
        Color c = img.color;

        if (onEraser)
        {
            c = new Color(251 / 255f, 196 / 255f, 148 / 255f, 0.5f);
            img.color = c;
        }
        else
        {
            c = new Color(1f, 1f, 1f, 1f);
            img.color = c;
        }
    }
}