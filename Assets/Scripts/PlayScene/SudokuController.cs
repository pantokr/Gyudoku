using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuController : MonoBehaviour
{
    public GameObject passDialog;
    public GameObject playManager;
    public GameObject mainPanel;

    public CellManager cellManager;
    public MemoManager memoManager;
    public SudokuMaker sudokuMaker;

    public static int undoIndex = 0;
    public List<Tuple<int[,], int[,,]>> lateSudoku = new List<Tuple<int[,], int[,,]>>();

    private List<Vector2Int> list = new List<Vector2Int>();

    private int[,] sudoku;
    private int[,] fullSudoku;
    private int[,,] memoSudoku;

    private void Start()
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

    public bool IsEmptyCell(int y, int x)
    {
        if (sudoku[y, x] == 0)
        {
            return true;
        }
        return false;
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

    #region normal 스도쿠 무결성 검사 newval == 1~9
    public bool IsNewValueAvailableRow(int y, int x, int newVal)
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
    public bool IsNewValueAvailableCol(int y, int x, int newVal)
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
    public bool IsNewValueAvailableSG(int y, int x, int newVal)
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
    public void CheckNewValueNormal(int y, int x, int newVal)
    {
        list = new List<Vector2Int>();
        IsNewValueAvailableRow(y, x, newVal);
        IsNewValueAvailableCol(y, x, newVal);
        IsNewValueAvailableSG(y, x, newVal);

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

                    if (memoSudoku[rightNumber - 1, y, x] == 0)
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
    public List<Vector2Int> GetEmptyCellInRow(int y)
    {
        List<Vector2Int> empty = new List<Vector2Int>();
        for (int _x = 0; _x < 9; _x++)
        {
            if (sudoku[y, _x] == 0)
            {
                empty.Add(new Vector2Int(_x, y));
            }
        }
        return empty;
    }
    public List<Vector2Int> GetEmptyCellInCol(int x)
    {
        List<Vector2Int> empty = new List<Vector2Int>();
        for (int _y = 0; _y < 9; _y++)
        {
            if (sudoku[_y, x] == 0)
            {
                empty.Add(new Vector2Int(x, _y));
            }
        }
        return empty;
    }
    public List<Vector2Int> GetEmptyCellInSG(int y, int x) //
    {
        List<Vector2Int> empty = new List<Vector2Int>();
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                if (sudoku[_y, _x] == 0)
                {
                    empty.Add(new Vector2Int(_x, _y));
                }
            }
        }
        return empty;
    }
    #endregion

    #region 메모 데이터 반환
    public int[] GetMemoRow(int value, int y)
    {
        int[] row = new int[9];
        for (int _x = 0; _x < 9; _x++)
        {
            row[_x] = memoSudoku[value, y, _x];
        }
        return row;
    }

    public int[] GetMemoCol(int value, int x)
    {
        int[] col = new int[9];
        for (int _y = 0; _y < 9; _y++)
        {
            col[_y] = memoSudoku[value, _y, x];
        }
        return col;
    }

    public int[] GetMemoSG(int value, int y, int x)
    {
        int[] SG = new int[9];
        int cnt = 0;
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                SG[cnt++] = memoSudoku[value, _y, _x];
            }
        }
        return SG;
    }

    #endregion

    #region 기타
    public void RecordSudokuLog()
    {
        undoIndex = lateSudoku.Count;

        Tuple<int[,], int[,,]> tuple = new Tuple<int[,], int[,,]>((int[,])sudoku.Clone(), (int[,,])memoSudoku.Clone());
        lateSudoku.Add(tuple);
    }
    #endregion 
}