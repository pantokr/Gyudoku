using System;
using System.Linq;
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

    private List<GameObject> hintCell = new List<GameObject>();
    private bool breaker = false;
    private GameObject[,] objects;
    private GameObject[,,,] memoObjects;

    private int[,] sudoku;
    private void Start()
    {
        sudoku = SudokuManager.sudoku;
        objects = cellManager.GetObjects();
        memoObjects = memoManager.GetWholeMemoObjects();
    }

    public void RunHint()
    {
        breaker = false;
        if (Settings.PlayMode == 0)
        {
            RunBasicHint();
        }
        else
        {
            if (!sudokuController.IsNormalSudoku())
            {
                string[] _str = { "오류가 있습니다." };
                hintDialogManager.StartDialog(_str);
            }
        }
        if (breaker)
        {
            return;
        }

        FindFullHouse();
        if (breaker)
        {
            return;
        }

        FindHiddenSingle();
        if (breaker)
        {
            return;
        }

        FindCrossPointing();
        if (breaker)
        {
            return;
        }

        if (Settings.PlayMode == 0)
        {
            FindFromFullSudoku();
            if (breaker)
            {
                return;
            }
        }
        else
        {
            string[] str = { "더 이상 힌트가 없습니다.\n" +
                "(메모가 작동되고 있다면 다른 힌트를 얻을 수 있습니다.)"};
            hintDialogManager.StartDialog(str);
            return;
        }
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
                Vector2Int cell = sudokuController.GetEmptyCellInRow(_y)[0];
                int val = ev[0];
                breaker = true;

                //대사
                string[] str = { "풀 하우스", $"가로 행에 한 값 {ev[0]}만 비어 있습니다." };
                hintCell.Clear();
                hintCell.Add(null);
                hintCell.Add(objects[cell.y, cell.x]);

                //처방
                List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                toFill.Add(new Tuple<Vector2Int, int>(cell, val));

                hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                return;
            }
        }

        //col 검사
        for (int _x = 0; _x < 9; _x++)
        {
            var ev = sudokuController.GetEmptyValueInCol(_x);
            if (ev.Count == 1)
            {
                Vector2Int cell = sudokuController.GetEmptyCellInCol(_x)[0];
                int val = ev[0];
                breaker = true;

                //대사
                string[] str = { "풀 하우스", $"세로 열에 한 값 {ev[0]}만 비어 있습니다." };
                hintCell.Clear();
                hintCell.Add(null);
                hintCell.Add(objects[cell.y, cell.x]);

                //처방
                List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                toFill.Add(new Tuple<Vector2Int, int>(cell, val));

                hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                return;
            }
        }

        //서브그리드
        for (int _y = 0; _y < 3; _y++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                var ev = sudokuController.GetEmptyValueInSG(_y, _x);
                if (ev.Count == 1)
                {
                    Vector2Int cell = sudokuController.GetEmptyCellInSG(_y, _x)[0];
                    int val = ev[0];
                    breaker = true;

                    //대사
                    string[] str = { "풀 하우스", $"3X3 서브그리드에 한 값 {ev[0]}만 비어 있습니다." };
                    hintCell.Clear();
                    hintCell.Add(null);
                    hintCell.Add(objects[cell.y, cell.x]);

                    //처방
                    List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                    toFill.Add(new Tuple<Vector2Int, int>(cell, val));

                    hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                    return;
                }
            }
        }
    }

    public bool FindHiddenSingle(bool isAutoSingle = false)
    {
        //row 검사
        for (int val = 0; val < 9; val++)
        {
            for (int _y = 0; _y < 9; _y++)
            {
                var ev = sudokuController.GetMemoRow(val, _y);
                if (ev.Sum() == 1)
                {
                    breaker = true;
                    int _x = Array.IndexOf(ev, 1);

                    if (isAutoSingle)
                    {
                        cellManager.FillCell(_y, _x, val + 1);
                        return true;
                    }
                    else // 일반
                    {
                        //대사
                        string[] str = { "히든 싱글", $"가로 행에서 이 셀에 들어갈 수 있는 값은 {val + 1} 하나입니다." };
                        hintCell.Clear();
                        hintCell.Add(null);
                        hintCell.Add(objects[_y, _x]);

                        //처방
                        List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                        toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                        hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                        return true;

                    }
                }
            }
        }

        //col 검사
        for (int val = 0; val < 9; val++)
        {
            for (int _x = 0; _x < 9; _x++)
            {
                var ev = sudokuController.GetMemoCol(val, _x);
                if (ev.Sum() == 1)
                {
                    breaker = true;
                    int _y = Array.IndexOf(ev, 1);

                    if (isAutoSingle)
                    {
                        cellManager.FillCell(_y, _x, val + 1);
                        return true;
                    }
                    else // 일반
                    {
                        //대사
                        string[] str = { "히든 싱글", $"세로 열에서 이 셀에 들어갈 수 있는 값은 {val + 1} 하나입니다." };
                        hintCell.Clear();
                        hintCell.Add(null);
                        hintCell.Add(objects[_y, _x]);

                        //처방
                        List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                        toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                        hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                        return true;

                    }
                }
            }
        }

        //SG 검사
        for (int val = 0; val < 9; val++)
        {
            for (int _y = 0; _y < 3; _y++)
            {
                for (int _x = 0; _x < 3; _x++)
                {
                    var ev = sudokuController.GetMemoSG(val, _y, _x);
                    if (ev.Sum() == 1)
                    {
                        breaker = true;

                        if (isAutoSingle)
                        {
                            cellManager.FillCell(_y, _x, val + 1);
                            return true;
                        }
                        else // 일반
                        {
                            //대사
                            string[] str = { "히든 싱글", $"3X3 서브그리드에서 이 셀에 들어갈 수 있는 값은 {val + 1} 하나입니다." };
                            hintCell.Clear();
                            hintCell.Add(null);
                            hintCell.Add(objects[_y, _x]);

                            //처방
                            List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                            toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                            hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                            return true;

                        }
                    }
                }
            }
        }

        return false;
    } //히든 싱글

    public bool FindNakedSingle(bool isAutoSingle = false)
    {
        for (int _y = 0; _y < 9; _y++)
        {
            for (int _x = 0; _x < 9; _x++)
            {
                if (sudokuController.IsEmptyCell(_y, _x))
                {
                    List<int> vals = new List<int>();
                    for (int val = 0; val < 9; val++)
                    {
                        if (sudokuController.IsNewValueAvailableRow(_y, _x, val + 1) == true &&
                            sudokuController.IsNewValueAvailableCol(_y, _x, val + 1) == true &&
                            sudokuController.IsNewValueAvailableSG(_y, _x, val + 1) == true)
                        {
                            vals.Add(val);
                        }
                        if (vals.Count == 2)
                        {
                            break;
                        }
                    }
                    if (vals.Count == 1)
                    {
                        breaker = true;
                        int val = vals[0];

                        if (isAutoSingle)
                        {
                            cellManager.FillCell(_y, _x, val + 1);
                            return true;
                        }
                        else // 일반
                        {
                            //대사
                            string[] str = { "네이키드 싱글", $"이 셀에 {val + 1} 말고는 들어갈 수 있는 값이 없습니다." };
                            hintCell.Clear();
                            hintCell.Add(null);
                            hintCell.Add(objects[_y, _x]);

                            //처방
                            List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                            toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                            hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                            return true;

                        }
                    }
                }
            }
        }
        return false;
    } //다른 영역과의 교차점

    public void FindCrossPointing()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int val = 0; val < 9; val++)
                {
                    var (rows, cols) = sudokuController.GetLinesDisabledBySG(y, x, val);
                    foreach (var r_ in rows)
                    {

                        print($"y:{y + 1},x:{x + 1},val:{val + 1},r:{r_}");
                    }
                    if (rows.Count == 1)
                    {
                        int r = rows[0];
                        //print($"y:{y + 1},x:{x + 1},val:{val + 1},r:{r}");
                        List<int> crosses = new List<int>();
                        for (int _x = 0; _x < 9; _x++)
                        {
                            if (_x >= x * 3 && _x < x * 3 + 3)
                            {
                                continue;
                            }

                            if (sudoku[r, _x] == val)
                            {
                                crosses.Add(_x);
                            }
                        }

                        if (crosses.Count != 0)
                        {
                            breaker = true;
                            //대사

                            string[] str = { "교차점(Pointing)", $"{r + 1}행은  {y * 3 + x + 1}번 째 서브그리드 외엔 {val + 1} 값이 들어갈 수 없습니다. " };
                            hintCell.Clear();
                            hintCell.Add(null);

                            //처방
                            List<Tuple<Vector2Int, int>> toDelete = new List<Tuple<Vector2Int, int>>();
                            foreach (var cx in crosses)
                            {
                                toDelete.Add(new Tuple<Vector2Int, int>(new Vector2Int(cx, r), val + 1));
                            }

                            hintDialogManager.StartDialogAndDeleteMemo(str, hintCell, toDelete);


                            return;
                        }
                    }
                }
            }
        }
    }
    private void FindFromFullSudoku()
    {
        for (int _y = 0; _y < 9; _y++)
        {
            for (int _x = 0; _x < 9; _x++)
            {
                if (SudokuManager.sudoku[_y, _x] == 0)
                {
                    breaker = true;
                    int val = SudokuManager.fullSudoku[_y, _x];

                    //대사
                    string[] str = { "길라잡이", $"{_y + 1}행 {_x + 1}열의 값은 {val} 입니다." };

                    hintCell.Clear();
                    hintCell.Add(null);
                    hintCell.Add(objects[_y, _x]);

                    //처방
                    List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                    toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val));
                    hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                }
            }
        }
    }
}
