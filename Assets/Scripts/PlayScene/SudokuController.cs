using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuController : MonoBehaviour
{
    public CellManager cellManager;
    public MemoManager memoManager;

    public static int undoIndex = 0;
    public List<Tuple<int[,], bool[,,]>> lateSudoku = new List<Tuple<int[,], bool[,,]>>();

    private List<Vector2Int> list = new List<Vector2Int>();


    public bool isInCell(int y, int x, int value)
    {
        return cellManager.sudoku[y, x] == value;
    }
    public bool isInMemoCell(int y, int x, int value)
    {
        if (value == 0)
        {
            return false;
        }
        return memoManager.memoSudoku[value - 1, y, x] == true;
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
    #endregion
    public void RecordSudokuLog()
    {
        undoIndex = lateSudoku.Count;

        Tuple<int[,], bool[,,]> tuple = new Tuple<int[,], bool[,,]>(cellManager.GetSudoku(), memoManager.GetMemoSudoku());
        lateSudoku.Add(tuple);
    }
}
