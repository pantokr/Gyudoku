using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CellManager : MonoBehaviour
{
    public PlayManager playManager;
    public MemoManager memoManager;
    public SudokuController sudokuController;
    public FileManager fileManager;
    public AnimationClip twinkler;

    public Color normalColor;
    public Color highLightedColor;
    public Color pressedColor;
    public Color selectedColor;
    public Color disabledColor;
    public Color highLightCellColor;

    readonly private Button[,] btns = new Button[9, 9];
    readonly private Text[,] values = new Text[9, 9];
    readonly private GameObject[,] objects = new GameObject[9, 9];

    public int[,] sudoku;

    private void Awake()
    {
        LoadCells();
    }
    private void Start()
    {
        
        if (Settings.PlayMode == 1 || Settings.PlayMode == 2)
        {
            fileManager.StartOpening();
        }

        sudoku = SudokuManager.sudoku;
        InitCells();

        playManager.SetCur(-1, -1);
        SetButtonColor();
    }

    #region Get/Set sudoku data
    public Text[,] GetSudokuValueTexts()
    {
        return values;
    }
    public GameObject[,] GetObjects()
    {
        return objects;
    }
    public void ApplySudoku(int[,] s)
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                FillCell(y, x, s[y, x]);
            }
        }
    }
    #endregion

    #region Change cell status
    public void HighlightCells(int value)
    {
        string s = value.ToString();
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (String.Equals(s, values[y, x].text) ||
                    sudokuController.IsInMemoCell(y, x, value))// 하이라이트
                {
                    var colors = btns[y, x].colors;
                    colors.disabledColor = highLightCellColor;
                    colors.normalColor = highLightCellColor;
                    btns[y, x].colors = colors;
                }
                else //다른 숫자 원상복구
                {
                    var colors = btns[y, x].colors;
                    colors.disabledColor = disabledColor;
                    colors.normalColor = normalColor;
                    btns[y, x].colors = colors;
                }
            }
        }
    }
    public void FillCell(int y, int x, int value)
    {
        if (value == 0)
        {
            DeleteCell(y, x);
            return;
        }
        memoManager.DeleteMemoCell(y, x); //모든 메모 지우기
        memoManager.DeleteMemoCellsAtOnce(y, x, value); //해당 숫자 메모 지우기

        values[y, x].text = value.ToString();
        sudoku[y, x] = value;
    }
    public void DeleteCell(int y, int x)
    {
        values[y, x].text = "";
        sudoku[y, x] = 0;
    }

    public void Twinkle(int y, int x)
    {
        Animation anim = objects[y, x].GetComponent<Animation>();
        anim.Stop();
        anim.Play();
    }
    #endregion    

    private void InitCells()
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                var value = sudoku[y, x];
                if (value == 0) // 비워진 칸은 interacable true
                {
                    values[y, x].text = "";
                }
                else // 채워진 칸은 interactable false
                {
                    values[y, x].text = $"{sudoku[y, x]}";
                    btns[y, x].interactable = false;
                }

                // 애니메이션
            }
        }


    }
    //cells to array
    private void LoadCells()
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                string tString = $"R{y + 1}C{x + 1}";
                btns[y, x] = transform.Find(tString).GetComponent<Button>();
                values[y, x] = btns[y, x].transform.Find("Text").gameObject.GetComponent<Text>();
                objects[y, x] = transform.Find(tString).gameObject;

                int ty = y, tx = x;
                btns[y, x].onClick.AddListener(delegate { SelectCell(ty, tx); });
            }
        }
    }
    private void SelectCell(int y, int x)
    {
        playManager.SetCur(y, x);

        if (ManualToolsManager.onEraser)
        {
            DeleteCell(y, x);
            memoManager.DeleteMemoCell(y, x);
        }
    }
    private void SetButtonColor()
    {
        //버튼 색깔 일괄 바꾸기
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                var colors = btns[y, x].colors;
                colors.normalColor = normalColor;
                colors.highlightedColor = highLightedColor;
                colors.pressedColor = pressedColor;
                colors.selectedColor = selectedColor;
                colors.disabledColor = disabledColor;

                btns[y, x].colors = colors;
            }
        }
    }
}
