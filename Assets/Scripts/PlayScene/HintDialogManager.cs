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
    public MemoManager memoManager;
    public HintLineManager hintLineManager;
    public SudokuController sudokuController;

    private Animation pusher;
    private string[] texts;

    private List<GameObject> hintCells = null;
    private List<List<GameObject>> hintCellsList = null;
    private List<Tuple<GameObject, GameObject>> hintLines = null;

    private List<Tuple<Vector2Int, int>> toFill = null;
    private List<GameObject> toDelete = null;

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

        this.hintCells = new List<GameObject>(hintCells);

        this.toFill = new List<Tuple<Vector2Int, int>>(toFill);


        StartDialog(texts);
    }
    public void StartDialogAndFillCell(string[] texts, List<List<GameObject>> hintCellsList, List<Tuple<Vector2Int, int>> toFill) // 오버라이드
    {

        this.texts = (string[])texts.Clone();

        this.hintCellsList = new List<List<GameObject>>(hintCellsList);

        this.toFill = null;

        this.toFill = new List<Tuple<Vector2Int, int>>(toFill);


        StartDialog(texts);
    }
    public void StartDialogAndDeleteMemo(string[] texts, List<GameObject> hintCells, List<GameObject> toDelete) //line
    {
        this.hintCells = new List<GameObject>(hintCells);
        this.toDelete = new List<GameObject>(toDelete);

        cur = 0;

        StartDialog(texts);
    }
    public void StartDialogAndDeleteMemo(string[] texts, List<List<GameObject>> hintCellsList, List<GameObject> toDelete) // 오버라이드
    {
        this.hintCellsList = new List<List<GameObject>>(hintCellsList);
        this.toDelete = new List<GameObject>(toDelete);

        cur = 0;

        StartDialog(texts);
    }

    public void ChangeText()
    {

        hintLineManager.EraseAllLine();
        hintLineManager.EraseAllCell();

        if (cur == texts.Length)
        {
            //end animation
            pusher.Stop("_Pusher");
            sudokuBoard.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            SetVisible(true);

            //대사화면 정리
            gameObject.SetActive(false);
            hintButton.GetComponent<Button>().enabled = true;

            //FillCell 처리
            if (toFill != null)
            {
                foreach (var l in toFill)
                {
                    cellManager.FillCell(l.Item1.y, l.Item1.x, l.Item2);
                }
                toFill = null;
            }

            //DeleteCell 처리
            if (toDelete != null)
            {
                foreach (var obj in toDelete)
                {
                    memoManager.DeleteMemoCell(obj);
                }
            }

            //null 처리
            hintCells = null;
            hintCellsList = null;
            hintLines = null;

            toFill = null;
            toDelete = null;

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

        if (hintCellsList != null) //필요 시 셀 강조
        {
            if (hintCellsList[cur] != null)
            {
                foreach (var hc in hintCellsList[cur])
                {
                    hintLineManager.HighlightCell(hc);
                }
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


