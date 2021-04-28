using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public SudokuController sudokuController;
    public DialogHint dialogHint;
    public AutoMemoManager autoMemoManager;
    public CellManager cellManager;
    public MemoManager memoManager;

    private bool breaker = false;
    public void RunHint()
    {
        breaker = false;

        RunBasicHint();

        print("ERROR");
    }

    public void RunBasicHint()
    {
        bool flag;
        List<Vector2Int> points;

        (flag, points) = sudokuController.CompareWithFullSudoku();
        if (flag)
        {
            breaker = true;
            //처방
            foreach (var point in points)
            {
                cellManager.DeleteCell(point.y, point.x);
            }

            string[] str = { "오류가 있습니다.", "스도쿠를 수정합니다." };
            dialogHint.StartDialog(str);
        }
        if (breaker)
        {
            return;
        }

        (flag, points) = sudokuController.CompareMemoWithFullSudoku();
        if (flag)
        {
            breaker = true;

            //처방
            autoMemoManager.RunAutoMemo();

            string[] str = { "메모가 불충분합니다.", "메모를 수정합니다." };
            dialogHint.StartDialog(str);
        }
        if (breaker)
        {
            return;
        }
    }
}
