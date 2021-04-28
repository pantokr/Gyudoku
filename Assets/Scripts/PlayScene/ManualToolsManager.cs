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
        if (Input.GetKeyDown(KeyCode.M))
        {
            TurnMemo();
            return;
        }

        //E을 누르면 Eraser on
        if (Input.GetKeyDown(KeyCode.E))
        {
            TurnEraser();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z))
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
    
    public void Undo()
    {
        if(SudokuController.undoIndex < 0)
        {
            return;
        }
        int[,] s = sudokuController.lateSudoku[SudokuController.undoIndex].Item1;
        bool[,,] m = sudokuController.lateSudoku[SudokuController.undoIndex].Item2;

        cellManager.ApplySudoku(s);

        memoManager.ApplyMemoSudoku(m);

        SudokuController.undoIndex--;
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