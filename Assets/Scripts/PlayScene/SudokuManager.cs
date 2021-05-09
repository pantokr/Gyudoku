using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuManager : MonoBehaviour
{
    public static int[,] sudoku;
    public static int[,] fullSudoku;
    public static int[,] originalSudoku;

    public static int[,,] memoSudoku;

    public FileManager fileManager;
    private SudokuMaker sudokuMaker;
    private void Awake()
    {
        memoSudoku = new int[9, 9, 9];
        sudokuMaker = new SudokuMaker();

        if (Settings.PlayMode == 0)
        {
            (sudoku, fullSudoku) = sudokuMaker.MakeNewSudoku();
            originalSudoku = (int[,])sudoku.Clone();
        }
        else if (Settings.PlayMode == 1 || Settings.PlayMode == 2)
        {
            fileManager.StartOpening();
        }
        else if (Settings.PlayMode == 3)
        {
            if (DifficultySetter.Difficulty == 0)
            {
                int rand = Random.Range(0, Settings.Easy_Cnt);
                fileManager.StartOpening($"easy{rand}");
                sudoku = sudokuMaker.MakeConvertedSudoku(sudoku);
            }
            else if (DifficultySetter.Difficulty == 1)
            {
                int rand = Random.Range(0, Settings.Medium_Cnt);
                fileManager.StartOpening($"medium{rand}");
                sudoku = sudokuMaker.MakeConvertedSudoku(sudoku);
            }
            else if (DifficultySetter.Difficulty == 2)
            {
                int rand = Random.Range(0, Settings.Hard_Cnt);
                fileManager.StartOpening($"hard{rand}");
                sudoku = sudokuMaker.MakeConvertedSudoku(sudoku);
            }
        }
    }
    public static void PrintSudoku(int[,] s)
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
    public static void PrintMemoYX(int y, int x)
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
    public static void PrintMemoVal(int val)
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

    public static void PrintListInt(List<int> list)
    {
        string str = "";
        foreach (var l in list)
        {
            str += $"{l} ";
        }
        print(str);
    }
}
