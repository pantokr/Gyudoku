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
    public HintVisualManager hintVisualManager;
    public SudokuController sudokuController;

    private Animation pusher;
    private string[] texts;

    private List<GameObject> hc = null;
    private List<List<GameObject>> hcList = null;
    private List<List<int>> hbList = null;
    private List<List<(GameObject, GameObject)>> hlList = null;

    private Tuple<(int, int), int> toFill = null;
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
    public void StartDialogAndFillCell(string[] texts, List<GameObject> hc, Tuple<(int, int), int> toFill, List<List<int>> hbList = null) //
    {

        this.texts = (string[])texts.Clone();

        this.hc = new List<GameObject>(hc);

        this.toFill = new Tuple<(int, int), int>((toFill.Item1.Item1, toFill.Item1.Item2), toFill.Item2);

        if (hbList != null)
        {
            this.hbList = hbList;
        }

        StartDialog(texts);
    }
    public void StartDialogAndFillCell(string[] texts, List<List<GameObject>> hcList, Tuple<(int, int), int> toFill, List<List<int>> hbList = null) // 오버라이드
    {

        this.texts = (string[])texts.Clone();

        this.hcList = new List<List<GameObject>>(hcList);

        this.toFill = new Tuple<(int, int), int>((toFill.Item1.Item1, toFill.Item1.Item2), toFill.Item2);

        if (hbList != null)
        {
            this.hbList = hbList;
        }

        StartDialog(texts);
    }
    public void StartDialogAndDeleteMemo(string[] texts, List<GameObject> hc, List<GameObject> toDelete, List<List<int>> hbList = null) //line
    {
        this.hc = new List<GameObject>(hc);

        this.toDelete = new List<GameObject>(toDelete);

        if (hbList != null)
        {
            this.hbList = hbList;
        }

        StartDialog(texts);
    }
    public void StartDialogAndDeleteMemo(string[] texts, List<List<GameObject>> hcList, List<GameObject> toDelete, List<List<int>> hbList = null) // 오버라이드
    {
        this.hcList = new List<List<GameObject>>(hcList);

        this.toDelete = new List<GameObject>(toDelete);

        if (hbList != null)
        {
            this.hbList = hbList;
        }

        StartDialog(texts);
    }

    public void StartDialogAndDeleteMemo(string[] texts, List<List<GameObject>> hcList, List<GameObject> toDelete, List<List<(GameObject, GameObject)>> hlList, List<List<int>> hbList = null) // 오버라이드
    {
        this.hcList = new List<List<GameObject>>(hcList);

        this.toDelete = new List<GameObject>(toDelete);

        this.hlList = new List<List<(GameObject, GameObject)>>(hlList);

        if (hbList != null)
        {
            this.hbList = hbList;
        }

        StartDialog(texts);
    }

    public void ChangeText()
    {

        hintVisualManager.EraseAllLine();
        hintVisualManager.EraseAllCell();

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
                cellManager.FillCell(toFill.Item1.Item1, toFill.Item1.Item2, toFill.Item2);

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
            hc = null;
            hcList = null;
            hlList = null;
            hbList = null;

            toFill = null;
            toDelete = null;

            //게임 종료 처리
            sudokuController.FinishSudoku();

            return;
        }
        //text 변경
        _dialogText.text = texts[cur];

        if (hlList != null) //필요 시 힌트라인 생성
        {
            if (hlList[cur] != null)
            {
                foreach (var hl in hlList[cur])
                {
                    hintVisualManager.DrawLine(hl.Item1, hl.Item2);
                }
            }
        }

        if (hbList != null)
        {
            if (hbList[cur] != null)
            {
                foreach (var hb in hbList[cur])
                {
                    hintVisualManager.HighlightBundle(hb);
                }
            }
        }

        if (hc != null) //필요 시 셀 강조
        {
            if (hc[cur] != null)
            {
                hintVisualManager.HighlightCell(hc[cur]);
            }
        }

        if (hcList != null) //필요 시 셀 강조
        {
            if (hcList[cur] != null)
            {
                foreach (var hc in hcList[cur])
                {
                    hintVisualManager.HighlightCell(hc);
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


