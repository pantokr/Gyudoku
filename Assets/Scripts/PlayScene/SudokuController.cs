using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SudokuController : MonoBehaviour
{
    public GameObject passDialog;
    public GameObject playManager;
    public GameObject mainPanel;

    public CellManager cellManager;
    public MemoManager memoManager;
    public SudokuMaker sudokuMaker;


    public int[,] sudoku;
    public int[,] fullSudoku;
    public int[,,] memoSudoku;

    private List<Tuple<int[,], int[,,]>> lateSudoku = new List<Tuple<int[,], int[,,]>>();
    private int undoIndex = -1;

    protected virtual void Start()
    {
        sudoku = SudokuManager.sudoku;
        fullSudoku = SudokuManager.fullSudoku;
        memoSudoku = SudokuManager.memoSudoku;
    }

    #region 값 유무 확인
    public bool IsInCell(int y, int x, int value)
    {
        return sudoku[y, x] == value;
    }

    public bool IsInMemoCell(int y, int x, int value)
    {
        if (value == 0)
        {
            return false;
        }
        return memoSudoku[value - 1, y, x] == 1;
    }

    #endregion

    #region 두 셀 비교
    public bool IsEqualMemoCell((int, int) c1, (int, int) c2)
    {
        var l1 = GetActiveMemoValue(c1.Item1, c1.Item2);
        var l2 = GetActiveMemoValue(c2.Item1, c2.Item2);
        if (l1.Count != l2.Count)
        {
            return false;
        }

        for (int pin = 0; pin < l1.Count; pin++)
        {
            if (l1[pin] != l2[pin])
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region 스도쿠 완성 여부 검사
    public bool IsCompleteRow(int y)
    {
        bool[] tuple = new bool[9];

        for (int x = 0; x < 9; x++)
        {
            int v = cellManager.sudoku[y, x];
            if (v != 0)
            {
                tuple[v - 1] = true;
            }
        }

        for (int index = 0; index < 9; index++)
        {
            if (tuple[index] == false)
            {
                return false;
            }
        }

        return true;
    }
    public bool IsCompleteCol(int x)
    {
        bool[] tuple = new bool[9];

        for (int y = 0; y < 9; y++)
        {
            int v = cellManager.sudoku[y, x];
            if (v != 0)
            {
                tuple[v - 1] = true;
            }
        }

        for (int index = 0; index < 9; index++)
        {
            if (tuple[index] == false)
            {
                return false;
            }
        }

        return true;
    }
    public bool IsCompleteSG(int y, int x)
    {
        bool[] tuple = new bool[9];

        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                int v = cellManager.sudoku[_y, _x];
                if (v != 0)
                {
                    tuple[v - 1] = true;
                }
            }
        }

        for (int index = 0; index < 9; index++)
        {
            if (tuple[index] == false)
            {
                return false;
            }
        }

        return true;
    }
    public bool IsSudokuComplete()
    {
        bool t;
        for (int y = 0; y < 9; y++)
        {
            t = IsCompleteRow(y);
            if (!t)
            {
                return false;
            }
        }

        for (int x = 0; x < 9; x++)
        {
            t = IsCompleteCol(x);
            if (!t)
            {
                return false;
            }
        }

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                t = IsCompleteSG(y, x);
                if (!t)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool FinishSudoku()
    {
        if (IsSudokuComplete())
        {
            passDialog.SetActive(true);
            playManager.SetActive(false);

            mainPanel.transform.Find("ManualTools").gameObject.SetActive(false);
            mainPanel.transform.Find("NumberHighlighter").gameObject.SetActive(false);
            mainPanel.transform.Find("AutoTools").gameObject.SetActive(false);
            mainPanel.transform.Find("Finisher").Find("MainMenuButton").gameObject.SetActive(true);
            mainPanel.transform.Find("Finisher").Find("SaveButton").gameObject.SetActive(false);

            return true;
        }
        return false;
    }
    #endregion

    #region 스도쿠 정상 여부 검사
    public bool IsNormalRow(int y)
    {
        List<int> nums = new List<int>();
        for (int _x = 0; _x < 9; _x++)
        {
            int val = sudoku[y, _x];
            if (val != 0 && nums.Contains(val))
            {
                return false;
            }
            else
            {
                nums.Add(val);
            }
        }
        return true;
    }
    public bool IsNormalCol(int x)
    {
        List<int> nums = new List<int>();
        for (int _y = 0; _y < 9; _y++)
        {
            int val = sudoku[_y, x];
            if (val != 0 && nums.Contains(val))
            {
                return false;
            }
            else
            {
                nums.Add(val);
            }
        }
        return true;
    }
    public bool IsNormalSG(int y, int x)
    {
        List<int> nums = new List<int>();
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                int val = sudoku[_y, _x];
                if (val != 0 && nums.Contains(val))
                {
                    return false;
                }
                else
                {
                    nums.Add(val);
                }
            }
        }
        return true;
    }

    public bool IsNormalSudoku()
    {
        for (int y = 0; y < 9; y++)
        {
            if (!IsNormalRow(y))
            {
                return false;
            }
        }

        for (int x = 0; x < 9; x++)
        {
            if (!IsNormalCol(x))
            {
                return false;
            }
        }
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (!IsNormalSG(y, x))
                {
                    return false;
                }
            }
        }

        return true;
    }
    #endregion

    #region 스도쿠 무결성 검사 newval == 1~9
    public bool IsNewValueAvailableRow(int y, int x, int newVal, List<(int, int)> list = null)
    {
        bool flag = true;
        for (int _x = 0; _x < 9; _x++)
        {
            if (x != _x && cellManager.sudoku[y, _x] == newVal)
            {
                if (list != null)
                {
                    list.Add((y, _x));
                }
                flag = false;
            }
        }
        return flag;
    }
    public bool IsNewValueAvailableCol(int y, int x, int newVal, List<(int, int)> list = null)
    {
        bool flag = true;
        for (int _y = 0; _y < 9; _y++)
        {
            if (y != _y && cellManager.sudoku[_y, x] == newVal)
            {
                if (list != null)
                {
                    list.Add((_y, x));
                }
                flag = false;
            }
        }
        return flag;
    }
    public bool IsNewValueAvailableSG(int y, int x, int newVal, List<(int, int)> list = null)
    {
        bool flag = true;
        int ty = y / 3;
        int tx = x / 3;
        for (int _y = ty * 3; _y < ty * 3 + 3; _y++)
        {
            for (int _x = tx * 3; _x < tx * 3 + 3; _x++)
            {
                if (y != _y && x != _x && cellManager.sudoku[_y, _x] == newVal)
                {
                    if (list != null)
                    {
                        list.Add((_y, _x));
                    }
                    flag = false;
                }
            }
        }
        return flag;
    }
    public void CheckNewValueNormal(int y, int x, int newVal)
    {
        List<(int, int)> list = new List<(int, int)>();
        IsNewValueAvailableRow(y, x, newVal, list);
        IsNewValueAvailableCol(y, x, newVal, list);
        IsNewValueAvailableSG(y, x, newVal, list);

        for (int index = 0; index < list.Count; index++)
        {
            cellManager.Twinkle(list[index].Item1, list[index].Item2);
        }
    }
    public List<(int, int)> CompareWithFullSudoku()
    {
        sudokuMaker = new SudokuMaker();

        List<(int, int)> points = new List<(int, int)>();
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (sudoku[y, x] != 0 &&
                    sudoku[y, x] != fullSudoku[y, x])
                {
                    points.Add((y, x));
                }
            }
        }
        return points;
    }
    public List<(int, int)> CompareMemoWithFullSudoku()
    {
        sudokuMaker = new SudokuMaker();

        List<(int, int)> points = new List<(int, int)>();
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (sudoku[y, x] == 0) // 스도쿠 값이 없을 때
                {
                    int rightNumber = fullSudoku[y, x];

                    if (memoSudoku[rightNumber - 1, y, x] == 0)
                    {
                        points.Add((y, x));
                    }
                }
            }
        }
        return points;
    }
    #endregion

    #region 빈 값 반환
    public List<int> GetEmptyValueInRow(int y)
    {
        int[] n = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> empty = new List<int>(n);
        for (int _x = 0; _x < 9; _x++)
        {
            if (sudoku[y, _x] != 0)
            {
                empty.Remove(sudoku[y, _x]);
            }
        }
        return empty;
    }
    public List<int> GetEmptyValueInCol(int x)
    {
        int[] n = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> empty = new List<int>(n);
        for (int _y = 0; _y < 9; _y++)
        {
            if (sudoku[_y, x] != 0)
            {
                empty.Remove(sudoku[_y, x]);
            }
        }
        return empty;
    }
    public List<int> GetEmptyValueInSG(int y, int x) //
    {
        int[] n = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> empty = new List<int>(n);
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                if (sudoku[_y, _x] != 0)
                {
                    empty.Remove(sudoku[_y, _x]);
                }
            }
        }
        return empty;
    }
    #endregion

    #region 빈 셀 반환
    public bool IsEmptyCell(int y, int x)
    {
        if (sudoku[y, x] == 0)
        {
            return true;
        }
        return false;
    }

    public List<int> GetEmptyCellsInRow(int y)
    {
        List<int> empty = new List<int>();
        for (int _x = 0; _x < 9; _x++)
        {
            if (IsEmptyCell(y, _x))
            {
                empty.Add(_x);
            }
        }
        return empty;
    }
    public List<int> GetEmptyCellsInCol(int x)
    {
        List<int> empty = new List<int>();
        for (int _y = 0; _y < 9; _y++)
        {
            if (IsEmptyCell(_y, x))
            {
                empty.Add(_y);
            }
        }
        return empty;
    }
    public List<(int, int)> GetEmptyCellsInSG(int y, int x) //
    {
        List<(int, int)> empty = new List<(int, int)>();
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                if (IsEmptyCell(_y, _x))
                {
                    empty.Add((_y, _x));
                }
            }
        }
        return empty;
    }
    #endregion

    #region 메모 데이터 반환
    public int[] GetMemoRow(int y, int value)
    {
        int[] row = new int[9];
        for (int _x = 0; _x < 9; _x++)
        {
            row[_x] = memoSudoku[value - 1, y, _x];
        }
        return row;
    }
    public int[] GetMemoCol(int x, int value)
    {
        int[] col = new int[9];
        for (int _y = 0; _y < 9; _y++)
        {
            col[_y] = memoSudoku[value - 1, _y, x];
        }
        return col;
    }
    public int[] GetMemoSG(int y, int x, int value)
    {
        int[] SG = new int[9];
        int cnt = 0;
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                SG[cnt++] = memoSudoku[value - 1, _y, _x];
            }
        }
        return SG;
    }
    #endregion

    #region 값이 존재하는 메모 셀 반환
    public List<int> GetMemoCellInRow(int y, int value)
    {
        List<int> list = new List<int>();
        for (int _x = 0; _x < 9; _x++)
        {
            if (IsInMemoCell(y, _x, value))
            {
                list.Add(_x);
            }
        }
        return list;
    }
    public List<int> GetMemoCellInCol(int x, int value)
    {
        List<int> list = new List<int>();
        for (int _y = 0; _y < 9; _y++)
        {
            if (IsInMemoCell(_y, x, value))
            {
                list.Add(_y);
            }
        }
        return list;
    }
    public List<(int, int)> GetMemoCellInSG(int y, int x, int value)
    {
        List<(int, int)> list = new List<(int, int)>();

        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                if (IsInMemoCell(_y, _x, value))
                {
                    list.Add((_y, _x));
                }
            }
        }
        return list;
    }

    #endregion

    #region 서브그리드의 셀에서 한 값이 차지하는 라인 영역 반환
    public (List<int>, List<int>) GetLinesDisabledBySG(int y, int x, int value) //서브그리드 좌표 매개변수, row, col 순으로 반환, 
    {
        List<int> rows = new List<int>();
        List<int> cols = new List<int>();
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                if (memoSudoku[value - 1, _y, _x] == 1)
                {
                    rows.Add(_y);
                    cols.Add(_x);
                }
            }
        }
        rows = rows.Distinct().ToList();
        rows.Sort();

        cols = cols.Distinct().ToList();
        cols.Sort();

        return (rows, cols); //끔찍해
    }

    public List<(int, int)> GetSGsDisbledByRow(int y, int value)
    {
        List<(int, int)> SGs = new List<(int, int)>();
        for (int _x = 0; _x < 9; _x++)
        {
            if (memoSudoku[value - 1, y, _x] == 1)
            {
                SGs.Add((y / 3, _x / 3));
            }
        }
        SGs = SGs.Distinct().ToList();
        SGs.Sort();

        return SGs;
    }

    public List<(int, int)> GetSGsDisbledByCol(int x, int value)
    {
        List<(int, int)> SGs = new List<(int, int)>();
        for (int _y = 0; _y < 9; _y++)
        {
            if (memoSudoku[value - 1, _y, x] == 1)
            {
                SGs.Add((_y / 3, x / 3));
            }
        }
        SGs = SGs.Distinct().ToList();
        SGs.Sort();

        return SGs;
    }

    #endregion

    #region 활성화된 메모값 반환
    public List<int> GetActiveMemoValue(int y, int x) //1~9
    {
        List<int> memoValueList = new List<int>();
        for (int val = 1; val <= 9; val++)
        {
            if (IsInMemoCell(y, x, val))
            {
                memoValueList.Add(val);
            }
        }
        return memoValueList;
    }
    public List<List<int>> GetMemoValuesInRow(int y)
    {
        List<List<int>> mv = new List<List<int>>();
        for (int _x = 0; _x < 9; _x++)
        {
            if (IsEmptyCell(y, _x))
            {
                mv.Add(GetActiveMemoValue(y, _x));
            }
        }
        return mv;
    }
    public List<List<int>> GetMemoValuesInCol(int x)
    {
        List<List<int>> mv = new List<List<int>>();
        for (int _y = 0; _y < 9; _y++)
        {
            if (IsEmptyCell(_y, x))
            {
                mv.Add(GetActiveMemoValue(_y, x));
            }
        }
        return mv;
    }
    public List<List<int>> GetMemoValuesInSG(int y, int x)
    {
        List<List<int>> mv = new List<List<int>>();
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                if (IsEmptyCell(_y, _x))
                {
                    mv.Add(GetActiveMemoValue(_y, _x));
                }
            }
        }
        return mv;
    }
    #endregion

    #region 두 셀의 겹치는 값들 반환
    public List<int> GetDuplicatedValueByTwoCell((int, int) c1, (int, int) c2)
    {
        List<int> list = new List<int>();
        var amv1 = GetActiveMemoValue(c1.Item1, c1.Item2);
        var amv2 = GetActiveMemoValue(c2.Item1, c2.Item2);

        foreach (var amv in amv1)
        {
            if (amv2.Contains(amv))
            {
                list.Add(amv);
            }
        }

        return list;
    }

    #endregion

    #region 링크된 셀들 반환
    public List<Tuple<int, int>> GetLinkedCell(int y, int x, int value, int preCode = -1)
    {
        List<Tuple<int, int>> link_list = new List<Tuple<int, int>>();

        //link_list[0] -> row, [1] -> col, [2] -> sg
        var mcr = GetMemoCellInRow(y, value);
        if (mcr.Count == 2 && preCode != 0)
        {
            Tuple<int, int> tuple = new Tuple<int, int>(y, (mcr[0] == x ? mcr[1] : mcr[0]));
            link_list.Add(tuple);
        }
        else
        {
            link_list.Add(null);
        }

        var mcc = GetMemoCellInCol(x, value);
        if (mcc.Count == 2 && preCode != 1)
        {
            Tuple<int, int> tuple = new Tuple<int, int>((mcc[0] == y ? mcc[1] : mcc[0]), x);
            link_list.Add(tuple);
        }
        else
        {
            link_list.Add(null);
        }

        var mcsg = GetMemoCellInSG(y / 3, x / 3, value);
        if (mcsg.Count == 2 && preCode != 2)
        {
            Tuple<int, int> tuple;

            if (mcsg[0].Item1 == y && mcsg[0].Item2 == x)
            {
                tuple = new Tuple<int, int>(mcsg[1].Item1, mcsg[1].Item2);
            }
            else
            {
                tuple = new Tuple<int, int>(mcsg[0].Item1, mcsg[0].Item2);
            }

            foreach (var l in link_list)
            {
                if (l != null && l.Item1 == tuple.Item1 && l.Item2 == tuple.Item2)
                {
                    tuple = null;
                    break;
                }
            }
            link_list.Add(tuple);
        }
        else
        {
            link_list.Add(null);
        }

        return link_list;
    }
    
    public List<Tuple<int, int>> GetLinkedCellRec(int y, int x, int value, int preCode = -1)
    {
        List<Tuple<int, int>> link_list = new List<Tuple<int, int>>();

        //link_list[0] -> row, [1] -> col, [2] -> sg
        var mcr = GetMemoCellInRow(y, value);
        if (mcr.Count == 2 && preCode != 0)
        {
            Tuple<int, int> tuple = new Tuple<int, int>(y, (mcr[0] == x ? mcr[1] : mcr[0]));
            link_list.Add(tuple);
        }
        else
        {
            link_list.Add(null);
        }

        var mcc = GetMemoCellInCol(x, value);
        if (mcc.Count == 2 && preCode != 1)
        {
            Tuple<int, int> tuple = new Tuple<int, int>((mcc[0] == y ? mcc[1] : mcc[0]), x);
            link_list.Add(tuple);
        }
        else
        {
            link_list.Add(null);
        }

        var mcsg = GetMemoCellInSG(y / 3, x / 3, value);
        if (mcsg.Count == 2 && preCode != 2)
        {
            Tuple<int, int> tuple;

            if (mcsg[0].Item1 == y && mcsg[0].Item2 == x)
            {
                tuple = new Tuple<int, int>(mcsg[1].Item1, mcsg[1].Item2);
            }
            else
            {
                tuple = new Tuple<int, int>(mcsg[0].Item1, mcsg[0].Item2);
            }

            foreach (var l in link_list)
            {
                if (l != null && l.Item1 == tuple.Item1 && l.Item2 == tuple.Item2)
                {
                    tuple = null;
                    break;
                }
            }
            link_list.Add(tuple);
        }
        else
        {
            link_list.Add(null);
        }

        return link_list;
    }

    #endregion

    #region 기타
    public void RecordSudokuLog()
    {
        cellManager.HighlightCells(0);

        Tuple<int[,], int[,,]> tuple = new Tuple<int[,], int[,,]>((int[,])sudoku.Clone(), (int[,,])memoSudoku.Clone());
        lateSudoku.Add(tuple);

        undoIndex++;
    }

    public (int[,], int[,,]) CallSudokuLog()
    {
        if (undoIndex < 0)
        {
            return (null, null);
        }
        var ret = ((int[,])lateSudoku[undoIndex].Item1.Clone(), (int[,,])lateSudoku[undoIndex].Item2.Clone());
        lateSudoku.RemoveAt(undoIndex);
        undoIndex--;
        return ret; ;
    }

    public int ValToY(int value)
    {
        return (value - 1) / 3;
    }

    public int ValToX(int value)
    {
        return (value - 1) % 3;
    }

    public int YXToVal(int y, int x)
    {
        return y * 3 + x;
    }
    #endregion 
}