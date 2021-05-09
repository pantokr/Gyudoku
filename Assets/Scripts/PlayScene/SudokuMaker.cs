using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuMaker
{
    private readonly int[,] sudokuSample = {
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
        { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
        { 2, 3, 1, 5, 6, 4, 8, 9, 7 },
        { 5, 6, 4, 8, 9, 7, 2, 3, 1 },
        { 8, 9, 7, 2, 3, 1, 5, 6, 4 },
        { 3, 1, 2, 6, 4, 5, 9, 7, 8 },
        { 6, 4, 5, 9, 7, 8, 3, 1, 2 },
        { 9, 7, 8, 3, 1, 2, 6, 4, 5 }
    };

    private readonly int EmptyC1 = DifficultySetter.EmptyC1;
    private readonly int EmptyC2 = DifficultySetter.EmptyC2;
    private readonly int EmptyMiddle = DifficultySetter.EmptyMiddle;
    private readonly int PatternCode = DifficultySetter.PatternCode;

    public int[,] fullSudoku;

    private MissingNumberMaker missingNumberMaker;
    private int[,] dst;

    public (int[,], int[,]) MakeNewSudoku() //sudoku, fullsudoku
    {
        //스도쿠 복사
        dst = (int[,])sudokuSample.Clone();

        for (int exCnt = 0; exCnt < 16; exCnt++) //16번 스도쿠 변형
        {
            int exCode = Random.Range(0, 8);
            ApplyRandomConversion(exCode);
        }

        fullSudoku = (int[,])dst.Clone();

        missingNumberMaker = new MissingNumberMaker(dst);
        missingNumberMaker.EmptySubGrid(EmptyC1, EmptyC2, EmptyMiddle, PatternCode);

        return ((int[,])dst.Clone(), (int[,])fullSudoku.Clone());
    }
    public int[,] MakeConvertedSudoku(int[,] old)
    {
        dst = (int[,])old.Clone();
        for (int exCnt = 0; exCnt < 16; exCnt++) //16번 스도쿠 변형
        {
            int exCode = Random.Range(0, 8);
            ApplyRandomConversion(exCode);
        }

        //for (int exCnt = 0; exCnt < 9; exCnt++)
        //{
        //    ApplyRandomConversion(7);
        //}

        return (int[,])dst.Clone();
    }

    private void ApplyRandomConversion(int code)
    {
        if (code == 0)
        {
            int tY = Random.Range(0, 3) * 3;
            SwapRow(tY, Random.Range(tY, tY + 3));
        }
        else if (code == 1)
        {
            int tX = Random.Range(0, 3) * 3;
            SwapCol(tX, Random.Range(tX, tX + 3));
        }
        else if (code == 2)
        {
            Swap3Rows(Random.Range(0, 3), Random.Range(0, 3));
        }
        else if (code == 3)
        {
            Swap3Cols(Random.Range(0, 3), Random.Range(0, 3));
        }
        else if (code == 4)
        {
            RotateSudoku90();
        }
        else if (code == 5)
        {
            MirrorRows();
        }
        else if (code == 6)
        {
            MirrorCols();
        }
        else if (code == 7)
        {
            int n1 = Random.Range(1, 10);
            int n2 = Random.Range(1, 10);

            if (n1 != n2)
            {
                SwapNumbers(n1, n2);
            }
        }
    }

    private void ApplyRandomConversion_Sub(int code)
    {

        if (code == 2)
        {
            Swap3Rows(Random.Range(0, 3), Random.Range(0, 3));
        }
        else if (code == 3)
        {
            Swap3Cols(Random.Range(0, 3), Random.Range(0, 3));
        }
        else if (code == 4)
        {
            RotateSudoku90();
        }
        else if (code == 5)
        {
            MirrorRows();
        }
        else if (code == 6)
        {
            MirrorCols();
        }
        else if (code == 7)
        {
            int n1 = Random.Range(1, 10);
            int n2 = Random.Range(1, 10);

            if (n1 != n2)
            {
                SwapNumbers(n1, n2);
            }
        }
    }

    #region sudoku conversion algorithms

    private void SwapRow(int y1, int y2)
    {
        int[] tRow = new int[9];
        for (int x = 0; x < 9; x++)
        {
            tRow[x] = dst[y1, x];
            dst[y1, x] = dst[y2, x];
            dst[y2, x] = tRow[x];
        }
    }

    private void SwapCol(int x1, int x2)
    {
        int[] tCol = new int[9];
        for (int y = 0; y < 9; y++)
        {
            tCol[y] = dst[y, x1];
            dst[y, x1] = dst[y, x2];
            dst[y, x2] = tCol[y];
        }
    }

    private void Swap3Rows(int y1, int y2)
    {
        int[,] tRows = new int[3, 9];
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                tRows[y, x] = dst[y1 * 3 + y, x];
                dst[y1 * 3 + y, x] = dst[y2 * 3 + y, x];
                dst[y2 * 3 + y, x] = tRows[y, x];
            }
        }
    }

    private void Swap3Cols(int x1, int x2)
    {
        int[,] tCols = new int[9, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                tCols[y, x] = dst[y, x1 * 3 + x];
                dst[y, x1 * 3 + x] = dst[y, x2 * 3 + x];
                dst[y, x2 * 3 + x] = tCols[y, x];
            }
        }
    }

    private void RotateSudoku90()
    {
        int[,] replica = new int[9, 9];
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                replica[x, 8 - y] = dst[y, x];
            }
        }
        dst = (int[,])replica.Clone();
    }

    private void MirrorRows()
    {
        int[] tRow = new int[9];

        for (int y = 0; y < 5; y++)
        {
            SwapRow(y, 8 - y);
        }
    }

    private void MirrorCols()
    {
        int[] tCol = new int[9];

        for (int x = 0; x < 5; x++)
        {
            SwapRow(x, 8 - x);
        }
    }

    private void SwapNumbers(int n1, int n2)
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (dst[y, x] == n1)
                {
                    dst[y, x] = -1;
                }

                if (dst[y, x] == n2)
                {
                    dst[y, x] = -2;
                }
            }

            for (int x = 0; x < 9; x++)
            {
                if (dst[y, x] == -1)
                {
                    dst[y, x] = n2;
                }

                if (dst[y, x] == -2)
                {
                    dst[y, x] = n1;
                }
            }
        }
    }

    #endregion

}
