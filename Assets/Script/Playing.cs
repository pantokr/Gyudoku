using System;
using UnityEngine;
using UnityEngine.UI;

public class Playing : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject PausePanel;
    public GameObject VictoryPanel;

    private GameObject numbers;
    private Button[,] buttons = new Button[9, 9];
    private Text[,] values = new Text[9, 9];

    public Color normalColor;
    public Color disabledColor;
    public Color highLightButtons;

    private SudokuMaker sudokuMaker;
    public static bool GameOver;
    //private Sudoku sudoku= new su;


    //현재 가리키고 있는 포인터
    private int curX;
    private int curY;

    private KeyCode[] numberKeys = // 1부터 9까지
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
                    values[y, x].text = "";
                }
                else
                {
                    values[y, x].text = $"{sudokuMaker.GetValue(y, x)}";
                    buttons[y, x].interactable = false;
                }
            }
        }

        //GameOver = false;
        curX = curY = -1;

    }

    private void LoadButtons()
    {
        numbers = MainPanel.transform.Find("Numbers").gameObject;

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                string toFind = $"R{y + 1}C{x + 1}";
                buttons[y, x] = numbers.transform.Find(toFind).GetComponent<Button>();

                AddButtonEvent(y, x);
                values[y, x] = buttons[y, x].transform.Find("Text").gameObject.GetComponent<Text>();
            }
        }
    }

    private void FixButtons()
    {
        Vector2 btnSize = new Vector2(96, 96);
        foreach (var btn in buttons)
        {
            btn.GetComponent<RectTransform>().sizeDelta = btnSize;
        }
    }

    private void AddButtonEvent(int y, int x)
    {
        int ny = y;
        int nx = x;
        buttons[y, x].onClick.AddListener(delegate { SelectButton(ny, nx); });
    }

    private void SelectButton(int y, int x)
    {
        curY = y;
        curX = x;
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
        if (Input.GetMouseButtonDown(0))
        {
            curY = 0;
            curX = 0;
        }

        for (int i = 0; i < numberKeys.Length; i++)
        {
            if (Input.GetKeyDown(numberKeys[i]))
            {
                if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
                {
                    values[curY, curX].text = (i + 1).ToString();
                }
                HighlightButtons(i + 1);
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
            {
                values[curY, curX].text = "";
            }
            HighlightButtons(0);
            return;
        }
    }

    private void HighlightButtons(int value)
    {
        string s = $"{value}";
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (String.Equals(s, values[y, x].text))
                {
                    var colors = buttons[y, x].colors;
                    colors.disabledColor = highLightButtons;
                    colors.normalColor = highLightButtons;
                    buttons[y, x].colors = colors;
                }
                else
                {
                    var colors = buttons[y, x].colors;
                    colors.disabledColor = disabledColor;
                    colors.normalColor = normalColor;
                    buttons[y, x].colors = colors;
                }
            }
        }
    }
}
