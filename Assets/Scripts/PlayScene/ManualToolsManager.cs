using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualToolsManager : MonoBehaviour
{
    public static bool onMemo = false;
    public static bool onEraser = false;

    //FB92C4 251, 196, 146\
    public SudokuController sudokuController;
    public MemoManager memoManager;
    public CellManager cellManager;
    public PlayManager playManager;

    public GameObject memoButton;
    public GameObject eraserButton;
    public GameObject undoButton;

    private Button _memo;
    private Button _eraser;
    private Button _undo;

    private Image memoImg;
    private Image eraserImg;

    private void Start()
    {
        _memo = memoButton.GetComponent<Button>();
        _eraser = eraserButton.GetComponent<Button>();
        _undo = undoButton.GetComponent<Button>();

        _memo.onClick.AddListener(delegate { TurnMemo(); });
        _eraser.onClick.AddListener(delegate { TurnEraser(); });
        _undo.onClick.AddListener(delegate { Undo(); });

        memoImg = memoButton.transform.GetComponent<Image>();
        eraserImg = eraserButton.transform.GetComponent<Image>();
    }
    private void Update()
    {

        //M을 누르면 Memo on
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TurnMemo();
            return;
        }

        //E을 누르면 Eraser on
        if (Input.GetKeyDown(KeyCode.E) && PlayManager.curX == -1 && PlayManager.curY == -1)
        {
            TurnEraser();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Undo();
            return;
        }
    }

    public void TurnMemo()
    {
        onMemo = !onMemo;
        if (onMemo)
        {
            onEraser = false;
            ApplyButtonPressed(memoImg);
            ApplyButtonNormal(eraserImg);

            GameObject.Find("MainPanel").GetComponent<Image>().color = new Color(0.8f, 1, 1, 1f);
        }
        else
        {
            ApplyButtonNormal(memoImg);
            GameObject.Find("MainPanel").GetComponent<Image>().color = new Color(1, 1, 1, 1);
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
            GameObject.Find("MainPanel").GetComponent<Image>().color = new Color(0.8f, 1, 1, 1);
        }
        else
        {
            ApplyButtonNormal(eraserImg);
            GameObject.Find("MainPanel").GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public void Undo()
    {
        cellManager.HighlightCells(0);
        var (s, m) = sudokuController.CallSudokuLog();

        if (s == null && m == null)
        {
            return;
        }

        cellManager.ApplySudoku(s);
        memoManager.ApplyMemoSudoku(m);
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