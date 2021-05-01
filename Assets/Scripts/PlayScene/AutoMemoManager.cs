using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMemoManager : MonoBehaviour
{
    public CellManager cellManager;
    public MemoManager memoManager;
    public SudokuController sudokuController;

    public void RunAutoMemo()
    {
        sudokuController.RecordSudokuLog();

        for (int val = 0; val < 9; val++)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (SudokuManager.sudoku[y, x] != 0)
                    {
                        continue;
                    }

                    if (sudokuController.IsNewValueAvailableRow(y, x, val + 1) &&
                        sudokuController.IsNewValueAvailableCol(y, x, val + 1) &&
                        sudokuController.IsNewValueAvailableSG(y, x, val + 1))
                    {
                        memoManager.FillMemoCell(y, x, val + 1);
                    }
                }
            }
        }
    }
}