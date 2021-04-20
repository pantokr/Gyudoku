using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuController : MonoBehaviour
{
    public CellManager cellManager;
    public MemoManager memoManager;

    private GameObject[,] objects;
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
}
