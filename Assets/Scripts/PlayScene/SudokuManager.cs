using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuManager : MonoBehaviour
{
    public static int[,] sudoku;
    public static int[,] fullSudoku;
    public static int[,] originalSudoku;

    public static int[,,] memoSudoku = new int[9, 9, 9];

    public FileManager fileManager;
    private SudokuMaker sudokuMaker;
    private void Awake()
    {
        if (Settings.PlayMode == 0)
        {
            sudokuMaker = new SudokuMaker();
            (sudoku, fullSudoku) = sudokuMaker.MakeNewSudoku();
            originalSudoku = (int[,])sudoku.Clone();
        }
        else if (Settings.PlayMode == 1 || Settings.PlayMode == 2)
        {
            fileManager.StartOpening();
        }
    }
    public static void printSudoku(int[,] s)
    {
        string str = "";
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                str += s[y, x].ToString();
                str += ' ';
            }
            str += '\n';
        }
        print(str);
    }
    public static void printMemoYX(int y, int x)
    {
        string str = "";
        for (int val = 0; val < 9; val++)
        {
            if (memoSudoku[val, y, x] == 1)
            {
                str += (val + 1).ToString();
            }
        }
        print(str);
    }
    public static void printMemoVal(int val)
    {
        string str = "";
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                str += memoSudoku[val - 1, y, x].ToString();
                str += ' ';
            }
            str += '\n';
        }
        print(str);
    }

}
