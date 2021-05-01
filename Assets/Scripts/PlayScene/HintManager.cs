using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public SudokuController sudokuController;
    public HintDialogManager hintDialogManager;
    public AutoMemoManager autoMemoManager;
    public CellManager cellManager;
    public MemoManager memoManager;

    private List<Tuple<GameObject, GameObject>> tmp_hint = new List<Tuple<GameObject, GameObject>>();
    private bool breaker = false;
    private GameObject[,] objects;
    private void Start()
    {
        objects = cellManager.GetObjects();
    }

    public void RunHint()
    {
        breaker = false;
        RunBasicHint();
        if (breaker)
        {
            return;
        }

        FindFullHouse();
        if (breaker)
        {
            return;
        }

        print("HINTMANAGER ERROR");
    }

    public void RunBasicHint()
    {
        bool flag;
        List<Vector2Int> points;

        // 오류 검사
        (flag, points) = sudokuController.CompareWithFullSudoku();
        if (flag)
        {
            breaker = true;

            //대사
            string[] str = { "오류가 있습니다.", "스도쿠를 수정합니다." };
            hintDialogManager.StartDialog(str);

            //처방
            foreach (var point in points)
            {
                cellManager.DeleteCell(point.y, point.x);
            }

        }
        if (breaker)
        {
            return;
        }

        //메모 충분 검사
        (flag, points) = sudokuController.CompareMemoWithFullSudoku();
        if (flag)
        {
            breaker = true;

            //대사
            string[] str = { "메모가 불충분합니다.", "메모를 수정합니다." };
            hintDialogManager.StartDialog(str);

            //처방
            autoMemoManager.RunAutoMemo();
        }
        if (breaker)
        {
            return;
        }
    }

    private void FindFullHouse() //한 셀만 부족하면
    {
        //row 검사
        for (int _y = 0; _y < 9; _y++)
        {
            var ev = sudokuController.GetEmptyValueInRow(_y);
            if (ev.Count == 1)
            {
                int x_cell = sudokuController.GetEmptyCellInRow(_y)[0];
                int x_val = ev[0];
                breaker = true;

                //대사
                string[] str = { "풀 하우스", "가로 행에 한 값만 비어 있습니다." };
                tmp_hint.Clear();
                tmp_hint.Add(null);
                tmp_hint.Add(new Tuple<GameObject, GameObject>(objects[_y, x_cell], objects[_y, x_cell]));
                hintDialogManager.StartDialog(str, tmp_hint);

                //처방
                cellManager.FillCell(_y, x_cell, x_val);
                return;
            }
        }
        
        //col 검사
        for (int _x = 0; _x < 9; _x++)
        {
            var ev = sudokuController.GetEmptyValueInCol(_x);
            if (ev.Count == 1)
            {
                int y_cell = sudokuController.GetEmptyCellInCol(_x)[0];
                int y_val = ev[0];
                breaker = true;

                //대사
                string[] str = { "풀 하우스", "세로 행에 한 값만 비어 있습니다." };
                tmp_hint.Clear();
                tmp_hint.Add(null);
                tmp_hint.Add(new Tuple<GameObject, GameObject>(objects[y_cell, _x], objects[y_cell, _x]));
                hintDialogManager.StartDialog(str, tmp_hint);

                //처방
                cellManager.FillCell(y_cell, _x, y_val);
                return;
            }
        }
    }
}
