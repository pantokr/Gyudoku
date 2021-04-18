using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject pausePanel;
    public CellManager cellManager;
    public MemoManager memoManager;
    public ManualToolButtonsManager manualToolButtonsManager;
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
        values = cellManager.GetSudokuValueTexts();

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
            cellManager.HighlightCells(0);
        }

        for (int i = 0; i < numberKeys.Length; i++)
        {
            if (Input.GetKeyDown(numberKeys[i]))
            {
                if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
                {
                    if (ManualToolButtonsManager.onMemo)
                    {
                        cellManager.DeleteCell(curY, curX);
                        memoManager.FillMemoCell(curY, curX, i + 1);
                    }
                    else
                    {
                        memoManager.DeleteMemoCell(curY, curX);
                        cellManager.FillCell(curY, curX, i + 1);
                        cellManager.HighlightCells(i + 1);
                    }
                }
                return;
            }
        }

        //Q를 누르면 지워짐
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
            {
                cellManager.DeleteCell(curY, curX);
                memoManager.DeleteMemoCell(curY, curX);
                cellManager.HighlightCells(0);
            }
            return;
        }

        //M을 누르면 Memo on
        if (Input.GetKeyDown(KeyCode.M))
        {
            manualToolButtonsManager.TurnMemo();
            return;
        }

        //E을 누르면 Memo on
        if (Input.GetKeyDown(KeyCode.E))
        {
            manualToolButtonsManager.TurnEraser();
            return;
        }
    }
    public void SetCur(int curY, int curX)
    {
        this.curY = curY;
        this.curX = curX;
    }

}