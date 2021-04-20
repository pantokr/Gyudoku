using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuController : MonoBehaviour
{
    public CellManager cellManager;
    public MemoManager memoManager;

    private GameObject[,] objects;
    private int[,] sudoku;

    public bool isInCell(int y, int x, int value)
    {
        return cellManager.GetSudokuValue(y, x) == value;
    }

    public bool isInMemoCell(int y, int x, int value)
    {
        GameObject obj = memoManager.GetMemoObject(y, x, value);
        if (!obj)
        {
            return false;
        }
        return obj.activeSelf;
    }
    public bool CheckRow(int y)
    {
        bool[] tuple = new bool[9];

        for (int x = 0; x < 9; x++)
        {
            int v = sudoku[y, x];
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
    public bool CheckCol(int x)
    {
        bool[] tuple = new bool[9];

        for (int y = 0; y < 9; y++)
        {
            int v = sudoku[y, x];
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
    public bool CheckSG(int y, int x)
    {
        bool[] tuple = new bool[9];

        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                int v = sudoku[_y, _x];
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
        sudoku = cellManager.sudoku;
        bool t;
        for (int y = 0; y < 9; y++)
        {
            t = CheckRow(y);
            if (!t)
            {
                return false;
            }
        }

        for (int x = 0; x < 9; x++)
        {
            t = CheckCol(x);
            if (!t)
            {
                return false;
            }
        }

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                t = CheckSG(y, x);
                if (!t)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
