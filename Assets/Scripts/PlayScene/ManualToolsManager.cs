using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualToolsManager : MonoBehaviour
{
    public static bool onMemo = false;
    public static bool onEraser = false;

    //FB92C4 251, 196, 146
    public GameObject memoButton;
    public GameObject eraserButton;
    
    private Button _memo;
    private Button _eraser;

    private Image memoImg;
    private Image eraserImg;
    private void Start()
    {
        _memo = memoButton.GetComponent<Button>();
        _eraser = eraserButton.GetComponent<Button>();

        _memo.onClick.AddListener(delegate { TurnMemo(); });
        _eraser.onClick.AddListener(delegate { TurnEraser(); });
        
        memoImg = memoButton.transform.GetComponent<Image>();
        eraserImg = eraserButton.transform.GetComponent<Image>();
    }

    public void TurnMemo()
    {
        onMemo = !onMemo;
        if (onMemo)
        {
            onEraser = false;
            ApplyButtonPressed(memoImg);
            ApplyButtonNormal(eraserImg);
        }
        else
        {
            ApplyButtonNormal(memoImg);
        }
    }

    public void TurnEraser()
    {
        onEraser = !onEraser;

        if (onEraser)
        {
            onMemo = false;
            ApplyButtonPressed(eraserImg);
            ApplyButtonNormal(memoImg);
        }
        else
        {
            ApplyButtonNormal(eraserImg);
        }
    }

    private void ApplyButtonPressed(Image img)
    {
        Color c = new Color(251 / 255f, 196 / 255f, 148 / 255f, 0.5f);
        img.color = c;
    }

    private void ApplyButtonNormal(Image img)
    {
        Color c = new Color(1f, 1f, 1f, 1f);
        img.color = c;
    }
}