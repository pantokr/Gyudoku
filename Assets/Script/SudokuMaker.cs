using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuMaker : MonoBehaviour
{
    readonly int[,] sudokuSample = {
        { 4, 6, 9, 5, 3, 7, 1, 2, 8 },
        { 5, 1, 7, 8, 2, 4, 6, 3, 9 },
        { 8, 3, 2, 9, 6, 1, 4, 7, 5 },
        { 1, 9, 6, 7, 5, 3, 2, 8, 4 },
        { 7, 5, 8, 4, 1, 2, 3, 9, 6 },
        { 2, 4, 3, 6, 8, 9, 5, 1, 7 },
        { 9, 7, 1, 2, 4, 5, 8, 6, 3 },
        { 3, 8, 4, 1, 7, 6, 9, 5, 2 },
        { 6, 2, 5, 3, 9, 8, 7, 4, 1 }
    };
    readonly int MissingNumberCnt = Settings.MissingNumberCnt;

    private int[,] sudoku;
    private void Start()
    {
        sudokuSample.Co
            pyTo(sudoku, 0);
    }
    private void MakeNewSudoku()
    {
    }

    public int GetValue(int y, int x)
    {
        return sudoku[y, x];
    }
}
