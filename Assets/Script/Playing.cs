using System;
using UnityEngine;
using UnityEngine.UI;

public class Playing : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject PausePanel;
    public GameObject VictoryPanel;

    private Button[,] cellButtons = new Button[9, 9];
    private Text[,] values = new Text[9, 9];

    public Color normalColor;
    public Color disabledColor;
    public Color highLightCells;

    private SudokuMaker sudokuMaker;
    //private NumberButtons numberButtons;
    // public static bool GameOver;

    //현재 가리키고 있는 포인터
    public int curX;
    private int curY;

    private KeyCode[] numberKeys = // 1부터 9까지
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
        KeyCode.Alpha8, KeyCode.Alpha9
    };

    private void Start()
    {
        // 배경화면 불러오기
        //MainPanel.GetComponent<Image>().sprite = Settings.Background;

        //스도쿠 만들기
        sudokuMaker = new SudokuMaker();

        // 버튼 담기
        LoadCells();
        //numberButtons.LoadNumberButtons();
        
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                var value = sudokuMaker.GetValue(y, x);
                if (value == 0) // 비워진 칸은 interacable true
                {
                    values[y, x].text = "";
                }
                else // 채워진 칸은 interactable false
                {
                    values[y, x].text = $"{sudokuMaker.GetValue(y, x)}";
                    cellButtons[y, x].interactable = false;
                }
            }
        }

        //GameOver = false;
        curX = curY = -1;

    }

    public void HighlightCells(int value)
    {
        string s = $"{value}";
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (String.Equals(s, values[y, x].text))
                {
                    var colors = cellButtons[y, x].colors;
                    colors.disabledColor = highLightCells;
                    colors.normalColor = highLightCells;
                    cellButtons[y, x].colors = colors;
                }
                else
                {
                    var colors = cellButtons[y, x].colors;
                    colors.disabledColor = disabledColor;
                    colors.normalColor = normalColor;
                    cellButtons[y, x].colors = colors;
                }
            }
        }
    }

    private void LoadCells()
    {
        GameObject cells = MainPanel.transform.Find("Cells").gameObject;

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                string tString = $"R{y + 1}C{x + 1}";
                cellButtons[y, x] = cells.transform.Find(tString).GetComponent<Button>();
                values[y, x] = cellButtons[y, x].transform.Find("Text").gameObject.GetComponent<Text>();

                int ty = y, tx = x;
                cellButtons[y, x].onClick.AddListener(delegate { SelectCell(ty, tx); });
            }
        }
    }
    private void SelectCell(int y, int x)
    {
        curY = y;
        curX = x;
    }


    // Update is called once per frame
    private void Update()
    {
        // esc pause
        if (Input.GetKeyDown(KeyCode.Escape))
            PausePanel.SetActive(true);
    }

    private void LateUpdate()
    {
        //클릭 시 일단 초기화
        if (Input.GetMouseButtonDown(0))
        {
            curY = -1;
            curX = -1;
            HighlightCells(0);
        }

        for (int i = 0; i < numberKeys.Length; i++)
        {
            if (Input.GetKeyDown(numberKeys[i]))
            {
                if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
                {
                    values[curY, curX].text = (i + 1).ToString();
                }
                HighlightCells(i + 1);
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
            {
                values[curY, curX].text = "";
            }
            HighlightCells(0);
            return;
        }
    }
}