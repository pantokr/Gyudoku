using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSingleManager : MonoBehaviour
{
    public HintManager hintManager;
    public HintDialogManager hintDialogManager;
    public SudokuController sudokuController;
    public MultipurposeDialogManager multipurposeDialogManager;
    // Start is called before the first frame update
    public void RunAutoSingle()
    {
        sudokuController.RecordSudokuLog();

        int cnt = 0;
        while (true)
        {
            if (cnt >= 81)
            {
                print("AUTOSINGLEMANAGER ERROR");
                return;
            }
            bool h = hintManager.FindHiddenSingle(true);
            bool n = hintManager.FindNakedSingle(true);

            if (h == false && n == false)
            {
                multipurposeDialogManager.RunDialog("Auto Single!");
                sudokuController.FinishSudoku();
                break;
            }
            cnt++;
        }
    }
}
