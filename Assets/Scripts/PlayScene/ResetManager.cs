using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject resetDialog;
    public GameObject submitButton;
    public GameObject cancelButton;

    public PlayManager playManager;
    public MemoManager memoManager;
    public CellManager cellManager;
    public SudokuController sudokuController;

    private Button _submit;
    private Button _cancel;

    private void Start()
    {
        _submit = submitButton.GetComponent<Button>();
        _cancel = cancelButton.GetComponent<Button>();

        _submit.onClick.AddListener(delegate { Submit(); });
        _cancel.onClick.AddListener(delegate { Cancel(); });
    }

    public void DisplayResetDialog()
    {
        resetDialog.SetActive(true);
        TurnObjects(false);
    }

    private void Cancel()
    {
        resetDialog.SetActive(false);
        TurnObjects(true);
    }

    private void Submit()
    {
        resetDialog.SetActive(false);
        UndoAll();
        TurnObjects(true);
    }
    public void UndoAll()
    {
        if (sudokuController.undoIndex == -1)
        {
            return;
        }
        cellManager.HighlightCells(0);

        cellManager.ApplySudoku(sudokuController.lateSudoku[0].Item1);
        memoManager.ApplyMemoSudoku(sudokuController.lateSudoku[0].Item2);

        sudokuController.lateSudoku.Clear();
        sudokuController.undoIndex = -1;
    }

    private void TurnObjects(bool onf)
    {
        playManager.enabled = onf;
        mainPanel.transform.Find("SudokuBoard").gameObject.SetActive(onf);
        mainPanel.transform.Find("NumberHighlighter").gameObject.SetActive(onf);
        mainPanel.transform.Find("ManualTools").gameObject.SetActive(onf);
        mainPanel.transform.Find("AutoTools").gameObject.SetActive(onf);
    }
}
