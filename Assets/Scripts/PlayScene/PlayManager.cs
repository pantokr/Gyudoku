using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject pausePanel;
    public CellManager cellManager;
    public MemoManager memoManager;
    public ManualToolsManager manualToolsManager;
    public NumberHighlighterManager numberHighlighterManager;
    public SudokuController sudokuController;
    public GameObject dialogPass;
    //public GameObject VictoryPanel;

    //현재 가리키고 있는 포인터
    private int curY;
    private int curX;

    private Text[,] values;

    private KeyCode[] AlphaKeys = // 1부터 9까지
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
        KeyCode.Alpha8, KeyCode.Alpha9
    };
    private KeyCode[] KeypadKeys = // 1부터 9까지
    {
        KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7,
        KeyCode.Keypad8, KeyCode.Keypad9
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
        {
            cellManager.HighlightCells(0);
            pausePanel.SetActive(true);
        }
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

        for (int i = 0; i < AlphaKeys.Length; i++)
        {
            if (Input.GetKeyDown(AlphaKeys[i]) || Input.GetKeyDown(KeypadKeys[i]))
            {
                if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
                {
                    if (ManualToolsManager.onMemo) //memo on
                    {
                        if (sudokuController.isInMemoCell(curY, curX, i + 1)) //메모 지워주기
                        {
                            memoManager.DeleteMemoCell(curY, curX, i + 1);
                        }
                        else //메모 쓰기
                        {
                            cellManager.DeleteCell(curY, curX);
                            memoManager.FillMemoCell(curY, curX, i + 1);
                        }
                    }
                    else // memo off
                    {
                        if (sudokuController.isInCell(curY, curX, i + 1)) //숫자 지워주기
                        {
                            cellManager.DeleteCell(curY, curX);
                        }
                        else
                        {
                            memoManager.DeleteMemoCell(curY, curX); //숫자 쓰기
                            cellManager.FillCell(curY, curX, i + 1);
                            //Checker
                            if (sudokuController.isSudokuComplete())
                            {
                                dialogPass.SetActive(true);
                                return;
                            }

                            cellManager.HighlightCells(i + 1);
                        }
                    }
                }
                else // no cursor
                {
                    numberHighlighterManager.CallHighlightFunc(i + 1);
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
            manualToolsManager.TurnMemo();
            return;
        }

        //E을 누르면 Memo on
        if (Input.GetKeyDown(KeyCode.E))
        {
            manualToolsManager.TurnEraser();
            return;
        }
    }
    public void SetCur(int curY, int curX)
    {
        this.curY = curY;
        this.curX = curX;
    }

}