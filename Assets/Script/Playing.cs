using System;
using UnityEngine;
using UnityEngine.UI;

public class Playing : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject pausePanel;
    public SudokuController sudokuController;
    public Cells cells;
    //public GameObject VictoryPanel;

    //현재 가리키고 있는 포인터
    private int curY;
    private int curX;

    private Text[,] values;

    private KeyCode[] numberKeys = // 1부터 9까지
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
        KeyCode.Alpha8, KeyCode.Alpha9
    };

    private void Start()
    {
        values = cells.GetSudokuValueTexts();

        // 배경화면 불러오기
        //MainPanel.GetComponent<Image>().sprite = Settings.Background;
    }

    private void Update()
    {
        // esc pause
        if (Input.GetKeyDown(KeyCode.Escape))
            pausePanel.SetActive(true);
    }

    private void LateUpdate()
    {
        //클릭 시 일단 초기화
        if (Input.GetMouseButtonDown(0))
        {
            curY = -1;
            curX = -1;
            cells.HighlightCells(0);
        }

        for (int i = 0; i < numberKeys.Length; i++)
        {
            if (Input.GetKeyDown(numberKeys[i]))
            {
                if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
                {
                    values[curY, curX].text = (i + 1).ToString();
                    cells.HighlightCells(i + 1);
                }
                return;
            }
        }

        //Q를 누르면 지워짐
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
            {
                values[curY, curX].text = "";
            }
            return;
        }
    }
    public void SetCur(int curY, int curX)
    {
        this.curY = curY;
        this.curX = curX;
    }

}