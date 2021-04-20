using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualToolsManager_C : MonoBehaviour
{
    public static bool onEraser = false;

    //FB92C4 251, 196, 146
    public GameObject eraserButton;

    private Button _eraser;
    private Image eraserImg;
    private void Start()
    {
        _eraser = eraserButton.GetComponent<Button>();

        _eraser.onClick.AddListener(delegate { TurnEraser(); });
        
        eraserImg = eraserButton.transform.GetComponent<Image>();
    }

    public void TurnEraser()
    {
        onEraser = !onEraser;

        if (onEraser)
        {
            ApplyButtonPressed(eraserImg);
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