using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSingleManager : MonoBehaviour
{
    public HintManager hintManager;
    public SudokuController sudokuController;
    // Start is called before the first frame update
    public void RunAutoSingle()
    {
        sudokuController.RecordSudokuLog();

        int cnt = 0;
        while (true)
        {
            if( cnt >= 81)
            {
                print("AUTOSINGLEMANAGER ERROR");
                return;
            }
            bool h = hintManager.FindHiddenSingle(true);
            bool n = false;//hintManager.FindNakedSingle(true);

            if (h == false && n == false)
            {
                sudokuController.FinishSudoku();
                break;
            }
            cnt++;
        }
    }
}
