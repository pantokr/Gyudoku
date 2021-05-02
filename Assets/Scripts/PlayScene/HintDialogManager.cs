using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintDialogManager : MonoBehaviour
{

    public GameObject hintButton;
    public GameObject sudokuBoard;
    public GameObject mainPanel;
    public CellManager cellManager;
    public HintLineManager hintLineManager;
    public SudokuController sudokuController;

    private Animation pusher;
    private string[] texts;

    private List<GameObject> hintCells;
    private List<Tuple<GameObject, GameObject>> hintLines;

    private List<Tuple<Vector2Int, int>> toFill;
    private List<Tuple<Vector2Int, int>> toDelete;

    private Text _dialogText;
    private int cur = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeText();
        }
    }

    // hint button >> hint manager >> dialog
    public void StartDialog(string[] texts)
    {
        //start animation

        pusher = sudokuBoard.GetComponent<Animation>();
        pusher.Play("_Pusher");
        SetVisible(false);

        this.texts = (string[])texts.Clone();

        //대화 상자 켜기
        gameObject.SetActive(true);
        hintButton.GetComponent<Button>().enabled = false;
        _dialogText = transform.Find("DialogText").GetComponent<Text>();

        cur = 0; //현재 진행 상황 갱신

        ChangeText();
    }
    public void StartDialogAndFillCell(string[] texts, List<GameObject> hintCells, List<Tuple<Vector2Int, int>> toFill) //
    {

        this.texts = (string[])texts.Clone();
        if (hintCells == null)
        {
            this.hintCells = null;
        }
        else
        {
            this.hintCells = new List<GameObject>(hintCells);
        }

        if (toFill == null)
        {
            this.toFill = null;
        }
        else
        {
            this.toFill = new List<Tuple<Vector2Int, int>>(toFill);
        }

        StartDialog(texts);
    }
    public void StartDialogAndDeleteMemo(string[] texts, List<GameObject> hintCells, List<Tuple<Vector2Int, int>> toDelete) //line
    {

        if (hintCells == null)
        {
            this.hintCells = null;
        }
        else
        {
            this.hintCells = new List<GameObject>(hintCells);
        }

        if (toDelete == null)
        {
            this.toDelete = null;
        }
        else
        {
            this.toDelete = new List<Tuple<Vector2Int, int>>(toDelete);
        }

        cur = 0;

        StartDialog(texts);
    }

    public void ChangeText()
    {
        if (cur == texts.Length)
        {
            //end animation
            pusher.Stop("_Pusher");
            sudokuBoard.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            SetVisible(true);

            //정리
            gameObject.SetActive(false);
            hintButton.GetComponent<Button>().enabled = true;

            hintLineManager.EraseAllLine();
            hintLineManager.EraseAllCell();

            //FillCell 처리
            if (toFill != null)
            {
                foreach (var l in toFill)
                {
                    cellManager.FillCell(l.Item1.y, l.Item1.x, l.Item2);
                }
                toFill = null;
            }

            //게임 종료 처리
            sudokuController.FinishSudoku();

            return;
        }
        //text 변경
        _dialogText.text = texts[cur];

        if (hintLines != null) //필요 시 힌트라인 생성
        {
            if (hintLines[cur] != null)
            {
                hintLineManager.DrawLine(hintLines[cur].Item1, hintLines[cur].Item2);
            }
        }

        if (hintCells != null) //필요 시 셀 강조
        {
            if (hintCells[cur] != null)
            {
                hintLineManager.HighlightCell(hintCells[cur]);
            }
        }
        cur++;
    }

    private void SetVisible(bool onf)
    {
        mainPanel.transform.Find("NumberHighlighter").gameObject.SetActive(onf);
        mainPanel.transform.Find("ManualTools").gameObject.SetActive(onf);
        mainPanel.transform.Find("Finisher").gameObject.SetActive(onf);
        mainPanel.transform.Find("AutoTools").gameObject.SetActive(onf);
    } //기타 오브젝트
}


