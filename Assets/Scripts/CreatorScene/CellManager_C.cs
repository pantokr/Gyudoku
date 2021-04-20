using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CellManager_C : MonoBehaviour
{
    public GameObject mainPanel;
    public PlayManager_C playManager;

    public Color normalColor; //FFFFFF 192
    public Color highLightedColor; //00317D 32
    public Color pressedColor; //00317D 64
    public Color selectedColor; //00317D 64
    public Color disabledColor; //FFFFFF 32
    public Color highLightCellColor; //F9DC5C 192

    readonly private Button[,] btns = new Button[9, 9];
    readonly private Text[,] values = new Text[9, 9];
    readonly private GameObject[,] objects = new GameObject[9, 9];

    private int[,] sudoku = new int[9, 9];

    private void Awake()
    {
        LoadCells();
        InitCells();

        playManager.SetCur(-1, -1);
    }
    private void Start()
    {
        //버튼 색깔 일괄 바꾸기
        SetButtonColor();
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
    public GameObject[,] GetObjects()
    {
        return objects;
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
                if (String.Equals(s, values[y, x].text))// 하이라이트
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
        values[y, x].text = value.ToString();
        sudoku[y, x] = value;
    }
    public void DeleteCell(int y, int x)
    {
        values[y, x].text = "";
        sudoku[y, x] = 0;
    }
    #endregion    

    private void InitCells()
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                int value = sudoku[y, x];
                if (value == 0) // 비워진 칸은 interacable true
                {
                    values[y, x].text = "";
                }
                else // 채워진 칸은 interactable false
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
                objects[y, x] = transform.Find(tString).gameObject;

                int ty = y, tx = x;
                btns[y, x].onClick.AddListener(delegate { SelectCell(ty, tx); });
            }
        }
    }


    private void SelectCell(int y, int x)
    {
        playManager.SetCur(y, x);

        if (ManualToolsManager_C.onEraser)
        {
            DeleteCell(y, x);
        }
    }

    private void SetButtonColor()
    {
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
