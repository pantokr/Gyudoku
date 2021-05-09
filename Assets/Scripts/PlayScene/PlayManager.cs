using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public GameObject pausePanel;
    public CellManager cellManager;
    public MemoManager memoManager;
    public NumberHighlighterManager numberHighlighterManager;
    public SudokuController sudokuController;

    //현재 가리키고 있는 포인터
    public int curY;
    public int curX;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
        }
        //클릭 시 일단 초기화

        #region arrowkeys
        if (Input.GetKeyDown(KeyCode.RightArrow) && curX < 9)
        {
            curX++;
            if (curX >= 9)
            {
                curX = 8;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && curX >= 0)
        {
            curX--;
            if (curX < 0)
            {
                curX = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && curY >= 0)
        {
            curY--;
            if (curY < 0)
            {
                curY = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && curY < 9)
        {
            curY++;
            if (curY >= 9)
            {
                curY = 8;
            }
        }
        #endregion


        if (Input.GetMouseButtonDown(0))
        {
            curY = -1;
            curX = -1;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            ManualToolsManager.onMemo = true;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ManualToolsManager.onMemo = false;
        }

        for (int i = 0; i < AlphaKeys.Length; i++)
        {
            if (Input.GetKeyDown(AlphaKeys[i]) || Input.GetKeyDown(KeypadKeys[i]))
            {
                if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
                {
                    sudokuController.RecordSudokuLog();
                    if (ManualToolsManager.onMemo) //memo on
                    {
                        if (sudokuController.IsInMemoCell(curY, curX, i + 1)) //메모 지워주기
                        {
                            memoManager.DeleteMemoCell(curY, curX, i + 1);
                            cellManager.HighlightCells(i + 1);
                        }
                        else //메모 쓰기
                        {
                            sudokuController.CheckNewValueNormal(curY, curX, i + 1); //정상 확인

                            cellManager.DeleteCell(curY, curX);
                            memoManager.FillMemoCell(curY, curX, i + 1);
                        }
                    }
                    else // memo off
                    {
                        if (sudokuController.IsInCell(curY, curX, i + 1)) //숫자 지워주기
                        {
                            cellManager.DeleteCell(curY, curX);
                            cellManager.HighlightCells(i + 1);
                        }
                        else
                        {
                            sudokuController.CheckNewValueNormal(curY, curX, i + 1); //정상 확인

                            cellManager.FillCell(curY, curX, i + 1);

                            //Checker
                            sudokuController.FinishSudoku();

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (curX != -1 && curY != -1) // 스도쿠 내부의 버튼을 선택하고 있으면
            {
                cellManager.DeleteCell(curY, curX);
                memoManager.DeleteMemoCell(curY, curX);
                cellManager.HighlightCells(0);
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