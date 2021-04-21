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

    #region complete
    public bool isCompleteRow(int y)
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
    public bool isCompleteCol(int x)
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
    public bool isCompleteSG(int y, int x)
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

    #region get/set
    public int[] GetRow(int y)
    {
        int[] res = new int[9];

        for (int x = 0; x < 9; x++)
        {
            res[x] = sudoku[y, x];
        }
        return res;
    }
    public int[] GetCol(int x)
    {
        int[] res = new int[9];

        for (int y = 0; y < 9; y++)
        {
            res[y] = sudoku[y, x];
        }
        return res;
    }
    public int[,] GetSG(int y, int x)
    {
        int[,] res = new int[3, 3];

        for (int _y = 0; _y < 3; _y++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                res[_y, _x] = sudoku[y * 3 + _y, x * 3 + _x];
            }
        }

        return res;
    }
    #endregion

    #region normal
    public void CheckNormalRow(int y, int x, int newVal, List<Vector2Int> list)
    {
        for (int _x = 0; _x < 9; _x++)
        {
            if (x != _x && sudoku[y, _x] == newVal)
            {
                list.Add(new Vector2Int(_x, y));
                //print("Row" + y.ToString() + _x.ToString());
            }
        }
    }
    public void CheckNormalCol(int y, int x, int newVal, List<Vector2Int> list)
    {
        for (int _y = 0; _y < 9; _y++)
        {
            if (y != _y && sudoku[_y, x] == newVal)
            {
                list.Add(new Vector2Int(x, _y));
                //print("Col" + _y.ToString() + x.ToString());

            }
        }
    }
    public void CheckNormalSG(int y, int x, int newVal, List<Vector2Int> list)
    {
        int ty = y / 3;
        int tx = x / 3;
        for (int _y = ty * 3; _y < ty * 3 + 3; _y++)
        {
            for (int _x = tx * 3; _x < tx * 3 + 3; _x++)
            {
                if (y != _y && x != _x && sudoku[_y, _x] == newVal)
                {
                    list.Add(new Vector2Int(_x, _y));
                    //print("SG" + _y.ToString() + _x.ToString());
                }
            }
        }
    }
    public void CheckNormal(int y, int x, int newVal)
    {
        sudoku = cellManager.sudoku;

        List<Vector2Int> list = new List<Vector2Int>();
        CheckNormalRow(y, x, newVal, list);
        CheckNormalCol(y, x, newVal, list);
        CheckNormalSG(y, x, newVal, list);

        for (int index = 0; index < list.Count; index++)
        {
            cellManager.Twinkle(list[index].y, list[index].x);
        }
    }
    #endregion
}
