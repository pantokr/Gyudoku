using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject PausePanel;
    public GameObject VictoryPanel;
    
    private Button[,] _buttons = new Button[9, 9];
    private Text[,] _values = new Text[9, 9];
    
    public Color normalColor;
    public Color disabledColor;
    public Color highLightButtons;
   
    
    public static bool GameOver;
    private Sudoku sudoku;

    private int _x;
    private int _y;

    private KeyCode[] _keys = // 1부터 9까지
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
        KeyCode.Alpha8, KeyCode.Alpha9
    };
    
    void Start()
    {
        MainPanel.GetComponent<Image>().sprite = Settings.Background;
        LoadButtons();
        FixButtons();
        sudoku = new Sudoku(Settings.Missing);
        sudoku.FillValues();
        for (int i = 0; i < 9; i++)
        {
            for (int x = 0; x < 9; x++)
            {
                var value = sudoku.GETValue(i, x);
                if (value == 0)
                {
                    _values[i, x].text = "-"; 
                }
                else
                {
                    _values[i, x].text = $"{sudoku.GETValue(i, x)}";
                    _buttons[i, x].interactable = false;
                }
            }
        }

        GameOver = false;
        _x = _y = -1;

    }

    private void LoadButtons()
    {
        for (int i = 0; i<9; i++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (i == 0)
                {
                    string toFind = $"0,{x}";
                    _buttons[i,x] = MainPanel.transform.Find(toFind).GetComponent<Button>();
                }
                else
                {
                    string toFind = $"0,{x} ({i})";
                    _buttons[i,x] = GameObject.Find(toFind).GetComponent<Button>();
                }

                AddButtonEvent(i, x);
                _values[i, x] = _buttons[i, x].transform.Find("Text").gameObject.GetComponent<Text>();
            }
        }
    }
    
    private void FixButtons()
    {
        Vector2 btnSize = new Vector2(95, 80);
        foreach (var btn in _buttons)
        {
            btn.GetComponent<RectTransform>().sizeDelta  = btnSize;
        }
    }

    private void AddButtonEvent(int x, int y)
    {
        int nx = x;
        int ny = y;
        _buttons[x,y].onClick.AddListener(delegate { SelectButton(nx,ny); });
    }

    private void SelectButton(int x, int y)
    {
        _x = x;
        _y = y;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            PausePanel.SetActive(true);
    }
    
    private void LateUpdate()
    {
        if(GameOver) return;
        for (int i=0; i<_keys.Length; i++)
        {
            if (Input.GetKeyDown(_keys[i]))
            {
                if (_x != -1 && _y != -1)
                {
                    _values[_x, _y].text = (i+1).ToString();
                    sudoku.SetValue(_x, _y, i + 1);
                    if (sudoku.IsCorrect(_x, _y))
                    {
                        CompleteCheck();
                    }
                }
                ButtonsHighlight(i+1);
                return;
            }
        }
    }

    private void ButtonsHighlight(int value)
    {
        string s = $"{value}";
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (String.Equals(s, _values[x, y].text))
                {
                    var colors = _buttons[x, y].colors;
                    colors.disabledColor = highLightButtons;
                    colors.normalColor = highLightButtons;
                    _buttons[x, y].colors = colors;
                }
                else
                {
                    var colors = _buttons[x, y].colors;
                    colors.disabledColor = disabledColor;
                    colors.normalColor = normalColor;
                    _buttons[x, y].colors = colors;
                }

                
            }
        }
    }

    private void CompleteCheck()
    {
        if (sudoku.Completed())
        {
            VictoryPanel.SetActive(true);
            GameOver = true;
        }
    }
}
