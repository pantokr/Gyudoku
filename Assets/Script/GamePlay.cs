using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject PausePanel;
    public GameObject VictoryPanel;

    private GameObject _numbers;
    private Button[,] _buttons = new Button[9, 9];
    private Text[,] _values = new Text[9, 9];

    public Color normalColor;
    public Color disabledColor;
    public Color highLightButtons;

    private SudokuMaker sudokuMaker;
    public static bool GameOver;
    //private Sudoku sudoku= new su;


    //현재 가리키고 있는 포인터
    private int _x;
    private int _y;

    private KeyCode[] _keys = // 1부터 9까지
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
        KeyCode.Alpha8, KeyCode.Alpha9
    };

    void Start()
    {
        // 배경화면 불러오기
        //MainPanel.GetComponent<Image>().sprite = Settings.Background;

        // 버튼 담기
        LoadButtons();
        // 모든 버튼 사이즈 변경
        FixButtons();

        sudokuMaker = new SudokuMaker();
        sudokuMaker.MakeNewSudoku();
        
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                var value = sudokuMaker.GetValue(y, x);
                if (value == 0)
                {
                    _values[y, x].text = "";
                }
                else
                {
                    _values[y, x].text = $"{sudokuMaker.GetValue(y, x)}";
                    _buttons[y, x].interactable = false;
                }
            }
        }

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                var value = sudokuMaker.GetValue(y, x);
                
            }
        }

        //GameOver = false;
        //_x = _y = -1;

    }

    private void LoadButtons()
    {
        _numbers = MainPanel.transform.Find("Numbers").gameObject;

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                string toFind = $"R{y + 1}C{x + 1}";
                _buttons[y, x] = _numbers.transform.Find(toFind).GetComponent<Button>();

                AddButtonEvent(y, x);
                _values[y, x] = _buttons[y, x].transform.Find("Text").gameObject.GetComponent<Text>();
            }
        }
    }

    private void FixButtons()
    {
        Vector2 btnSize = new Vector2(96, 96);
        foreach (var btn in _buttons)
        {
            btn.GetComponent<RectTransform>().sizeDelta = btnSize;
        }
    }

    private void AddButtonEvent(int y, int x)
    {
        int ny = y;
        int nx = x;
        _buttons[y, x].onClick.AddListener(delegate { SelectButton(ny, nx); });
    }

    private void SelectButton(int y, int x)
    {
        _y = y;
        _x = x;
    }

    // Update is called once per frame
    void Update()
    {
        // esc pause
        if (Input.GetKeyDown(KeyCode.Escape))
            PausePanel.SetActive(true);
    }

    private void LateUpdate()
    {
        //if (GameOver) return;
        //for (int i = 0; i < _keys.Length; i++)
        //{
        //    if (Input.GetKeyDown(_keys[i]))
        //    {
        //        if (_x != -1 && _y != -1)
        //        {
        //            _values[_x, _y].text = (i + 1).ToString();
        //            sudoku.SetValue(_x, _y, i + 1);
        //            if (sudoku.IsCorrect(_x, _y))
        //            {
        //                CompleteCheck();
        //            }
        //        }
        //        ButtonsHighlight(i + 1);
        //        return;
        //    }
        //}
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
        //if (sudoku.Completed())
        //{
        //    VictoryPanel.SetActive(true);
        //    GameOver = true;
        //}
    }
}
