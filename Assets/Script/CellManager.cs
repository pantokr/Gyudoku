using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CellManager : MonoBehaviour
{
    public GameObject mainPanel;
    public PlayManager playManager;
    public Color normalColor;
    public Color highLightedColor;
    public Color pressedColor;
    public Color selectedColor;
    public Color disabledColor;
    public Color highLightCellColor;

    readonly private Button[,] btns = new Button[9, 9];
    readonly private Text[,] values = new Text[9, 9];

    private SudokuMaker sudokuMaker;
    private int[,] sudoku;

    private void Awake()
    {
        sudokuMaker = new SudokuMaker();
        sudoku = sudokuMaker.MakeNewSudoku();

        LoadCells();
        InitCells();

        playManager.SetCur(-1, -1);
    }
    private void Start()
    {
        //¹öÆ° »ö±ò ÀÏ°ý ¹Ù²Ù±â
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

    #region Get sudoku data
    public int GetSudokuValue(int y, int x)
    {
        return sudoku[y, x];
    }
    public int[,] GetSudokuValues()
    {
        return sudoku;
    }
    public Text[,] GetSudokuValueTexts()
    {
        return values;
    }
    public Button[,] GetButtons()
    {
        return btns;
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
                if (String.Equals(s, values[y, x].text))
                {
                    var colors = btns[y, x].colors;
                    colors.disabledColor = highLightCellColor;
                    colors.normalColor = highLightCellColor;
                    btns[y, x].colors = colors;
                }
                else
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
        values[y, x].text = value.ToString();
    }
    public void DeleteCell(int y, int x)
    {
        values[y, x].text = "";
    }
    #endregion

    private void InitCells()
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                var value = GetSudokuValue(y, x);
                if (value == 0) // ºñ¿öÁø Ä­Àº interacable true
                {
                    values[y, x].text = "";
                }
                else // Ã¤¿öÁø Ä­Àº interactable false
                {
                    values[y, x].text = $"{GetSudokuValue(y, x)}";
                    btns[y, x].interactable = false;
                }
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

                int ty = y, tx = x;
                btns[y, x].onClick.AddListener(delegate { SelectCell(ty, tx); });
            }
        }
    }


    private void SelectCell(int y, int x)
    {
        playManager.SetCur(y, x);

        if (ManualToolButtons.onEraser)
        {
            DeleteCell(y, x);
        }
    }
}
