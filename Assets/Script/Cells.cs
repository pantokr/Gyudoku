using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Cells : MonoBehaviour
{
    public GameObject mainPanel;
    public SudokuController sudokuController;
    public Playing playing;
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
        FillCells();

        playing.SetCur(-1, -1);
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
    private void FillCells()
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
    private void SelectCell(int y, int x)
    {
        playing.SetCur(y, x);
    }
}
