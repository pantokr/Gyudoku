using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuController : MonoBehaviour
{
    public CellManager cellManager;
    public MemoManager memoManager;
    public SudokuMaker sudokuMaker;

    public static int undoIndex = 0;
    public List<Tuple<int[,], bool[,,]>> lateSudoku = new List<Tuple<int[,], bool[,,]>>();

    private List<Vector2Int> list = new List<Vector2Int>();

    private int[,] sudoku;
    private int[,] fullSudoku;
    private bool[,,] memoSudoku;

    private void Start()
    {
        sudoku = SudokuManager.sudoku;
        fullSudoku = SudokuManager.fullSudoku;
        memoSudoku = SudokuManager.memoSudoku;
    }

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
        return memoSudoku[value - 1, y, x] == true;
    }

    #region complete 스도쿠 완성 여부 검사
    public bool isCompleteRow(int y)
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
    public bool isCompleteCol(int x)
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
    public bool isCompleteSG(int y, int x)
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
    public bool isSudokuComplete()
    {
        bool t;
        for (int y = 0; y < 9; y++)
        {
            t = isCompleteRow(y);
            if (!t)
            {
                return false;
            }
        }

        for (int x = 0; x < 9; x++)
        {
            t = isCompleteCol(x);
            if (!t)
            {
                return false;
            }
        }

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                t = isCompleteSG(y, x);
                if (!t)
                {
                    return false;
                }
            }
        }

        return true;
    }

    #endregion

    #region normal 스도쿠 무결성 검사
    public bool isNormalRow(int y, int x, int newVal)
    {
        bool flag = true;
        for (int _x = 0; _x < 9; _x++)
        {
            if (x != _x && cellManager.sudoku[y, _x] == newVal)
            {
                list.Add(new Vector2Int(_x, y));
                //print("Row" + y.ToString() + _x.ToString());
                flag = false;
            }
        }
        return flag;
    }
    public bool isNormalCol(int y, int x, int newVal)
    {
        bool flag = true;
        for (int _y = 0; _y < 9; _y++)
        {
            if (y != _y && cellManager.sudoku[_y, x] == newVal)
            {
                list.Add(new Vector2Int(x, _y));
                //print("Col" + _y.ToString() + x.ToString());
                flag = false;
            }
        }
        return flag;
    }
    public bool isNormalSG(int y, int x, int newVal)
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
                    list.Add(new Vector2Int(_x, _y));
                    flag = false;
                    //print("SG" + _y.ToString() + _x.ToString());
                }
            }
        }
        return flag;
    }
    public void CheckNormal(int y, int x, int newVal)
    {
        list = new List<Vector2Int>();
        isNormalRow(y, x, newVal);
        isNormalCol(y, x, newVal);
        isNormalSG(y, x, newVal);

        for (int index = 0; index < list.Count; index++)
        {
            cellManager.Twinkle(list[index].y, list[index].x);
        }
    }
    public (bool, List<Vector2Int>) CompareWithFullSudoku()
    {
        sudokuMaker = new SudokuMaker();

        bool isWrong = false;
        List<Vector2Int> points = new List<Vector2Int>();
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (sudoku[y, x] != 0 &&
                    sudoku[y, x] != fullSudoku[y, x])
                {
                    points.Add(new Vector2Int(x, y));
                    isWrong = true;
                }
            }
        }
        return (isWrong, points);
    }
    public (bool, List<Vector2Int>) CompareMemoWithFullSudoku()
    {
        sudokuMaker = new SudokuMaker();

        bool isWrong = false;
        List<Vector2Int> points = new List<Vector2Int>();
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (sudoku[y, x] == 0) // 스도쿠 값이 없을 때
                {
                    int rightNumber = fullSudoku[y, x];
                    //print("right Number" + rightNumber.ToString());

                    if (memoSudoku[rightNumber - 1, y, x] == false)
                    {
                        points.Add(new Vector2Int(x, y));
                        isWrong = true;
                    }
                }
            }
        }
        return (isWrong, points);
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
    public List<int> GetEmptyCellInRow(int y)
    {
        List<int> empty = new List<int>();
        for (int _x = 0; _x < 9; _x++)
        {
            if (sudoku[y, _x] == 0)
            {
                empty.Add(_x);
            }
        }
        return empty;
    }
    public List<int> GetEmptyCellInCol(int x)
    {
        List<int> empty = new List<int>();
        for (int _y = 0; _y < 9; _y++)
        {
            if (sudoku[_y, x] == 0)
            {
                empty.Add(_y);
            }
        }
        return empty;
    }
    public List<Tuple<int, int>> GetEmptyCellInSG(int y, int x) //
    {
        List<Tuple<int, int>> empty = new List<Tuple<int, int>>();
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                if (sudoku[_y, _x] == 0)
                {
                    empty.Add(new Tuple<int, int>(_y, _x));
                }
            }
        }
        return empty;
    }
    #endregion

    public void RecordSudokuLog()
    {
        undoIndex = lateSudoku.Count;

        Tuple<int[,], bool[,,]> tuple = new Tuple<int[,], bool[,,]>((int[,])sudoku.Clone(), (bool[,,])memoSudoku.Clone());
        lateSudoku.Add(tuple);
    }
}