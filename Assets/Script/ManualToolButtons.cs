using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualToolButtons : MonoBehaviour
{
    public static bool onMemo = false;
    public static bool onEraser = false;

    public GameObject memoButton;
    public GameObject eraserButton;
    // Start is called before the first frame update
    void Start()
    {
        Button _memo = memoButton.GetComponent<Button>();
        Button _eraser = eraserButton.GetComponent<Button>();

        _memo.onClick.AddListener(delegate { TurnOnMemo(); });
        _eraser.onClick.AddListener(delegate { TurnOnEraser(); });
    }

    void TurnOnMemo()
    {
        onMemo = !onMemo;
        Debug.Log("Memo : " + onMemo);
    }

    void TurnOnEraser()
    {
        onEraser = !onEraser;
        Debug.Log("Eraser : " + onEraser);
    }
}