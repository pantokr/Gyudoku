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
            //ó��
            foreach (var point in points)
            {
                cellManager.DeleteCell(point.y, point.x);
            }

            string[] str = { "������ �ֽ��ϴ�.", "������ �����մϴ�." };
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

            //ó��
            autoMemoManager.RunAutoMemo();

            string[] str = { "�޸� ������մϴ�.", "�޸� �����մϴ�." };
            dialogHint.StartDialog(str);
        }
        if (breaker)
        {
            return;
        }
    }
}
