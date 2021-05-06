using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public SudokuController sudokuController;
    public HintDialogManager hintDialogManager;
    public AutoMemoManager autoMemoManager;
    public CellManager cellManager;
    public MemoManager memoManager;

    private bool breaker = false;
    private GameObject[,] objects;
    private GameObject[,,,] memoObjects;

    private void Start()
    {
        objects = cellManager.GetObjects();
        memoObjects = memoManager.GetWholeMemoObjects();
    }

    public void RunHint()
    {
        breaker = false;
        sudokuController.RecordSudokuLog();
        cellManager.HighlightCells(0);

        if (Settings.PlayMode == 0)
        {
            RunBasicHint();
        }
        else
        {
            if (!sudokuController.IsNormalSudoku())
            {
                string[] _str = { "오류가 있습니다." };
                hintDialogManager.StartDialog(_str);
            }
        }

        if (breaker)
        {
            return;
        }

        FindFullHouse();
        if (breaker)
        {
            return;
        }

        FindNakedSingle();
        if (breaker)
        {
            return;
        }

        FindHiddenSingle();
        if (breaker)
        {
            return;
        }

        FindCrossPointing();
        if (breaker)
        {
            return;
        }

        FindCrossClaiming();
        if (breaker)
        {
            return;
        }

        FindNakedPair();
        if (breaker)
        {
            return;
        }

        FindXWing();
        if (breaker)
        {
            return;
        }

        if (Settings.PlayMode == 0)
        {
            FindFromFullSudoku();
            if (breaker)
            {
                return;
            }
        }
        else
        {
            string[] str = { "더 이상 힌트가 없습니다.\n" +
                "(메모가 작동되고 있다면 다른 힌트를 얻을 수도 있습니다.)"};
            hintDialogManager.StartDialog(str);
            return;
        }
    }

    public void RunBasicHint()
    {
        // 오류 검사
        var p1 = sudokuController.CompareWithFullSudoku();
        if (p1.Count != 0)
        {
            breaker = true;

            //대사
            string[] str = { "오류가 있습니다.", "스도쿠를 수정합니다." };
            hintDialogManager.StartDialog(str);

            //처방
            foreach (var point in p1)
            {
                cellManager.DeleteCell(point.Item1, point.Item2);
            }

        }
        if (breaker)
        {
            return;
        }

        //메모 충분 검사
        var p2 = sudokuController.CompareMemoWithFullSudoku();
        if (p2.Count != 0)
        {
            breaker = true;

            //대사
            string[] str = { "메모가 불충분합니다.", "메모를 수정합니다." };
            hintDialogManager.StartDialog(str);

            //처방
            autoMemoManager.RunAutoMemo();
        }
        if (breaker)
        {
            return;
        }
    }

    private void FindFullHouse() //풀하우스
    {
        //row 검사
        for (int _y = 0; _y < 9; _y++)
        {
            var ev = sudokuController.GetEmptyValueInRow(_y);
            if (ev.Count == 1)
            {
                int _x = sudokuController.GetEmptyCellsInRow(_y)[0];
                int val = ev[0];
                breaker = true;

                //대사
                string[] str = { "풀 하우스", $"가로 행에 한 값 {ev[0]}만 비어 있습니다." };

                //처방
                var hc = MakeHC(null, objects[_y, _x]);
                var toFill = MakeTuple((_y, _x), val);

                hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                return;
            }
        }

        //col 검사
        for (int _x = 0; _x < 9; _x++)
        {
            var ev = sudokuController.GetEmptyValueInCol(_x);
            if (ev.Count == 1)
            {
                int _y = sudokuController.GetEmptyCellsInCol(_x)[0];
                int val = ev[0];
                breaker = true;

                //대사
                string[] str = { "풀 하우스", $"세로 열에 한 값 {ev[0]}만 비어 있습니다." };

                //처방
                var hc = MakeHC(null, objects[_y, _x]);
                var toFill = MakeTuple((_y, _x), val);

                hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                return;
            }
        }

        //서브그리드
        for (int _y = 0; _y < 3; _y++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                var ev = sudokuController.GetEmptyValueInSG(_y, _x);
                if (ev.Count == 1)
                {
                    var cell = sudokuController.GetEmptyCellsInSG(_y, _x)[0];
                    int val = ev[0];
                    breaker = true;

                    //대사
                    string[] str = { "풀 하우스", $"서브그리드에 한 값 {ev[0]}만 비어 있습니다." };

                    //처방
                    var hc = MakeHC(null, objects[_y, _x]);
                    var toFill = MakeTuple((_y, _x), val);

                    hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                    return;
                }
            }
        }
    }

    public bool FindNakedSingle(bool isAutoSingle = false) //네이키드 싱글
    {
        for (int _y = 0; _y < 9; _y++)
        {
            for (int _x = 0; _x < 9; _x++)
            {
                if (sudokuController.IsEmptyCell(_y, _x))
                {
                    var mv = sudokuController.GetActiveMemoValue(_y, _x);

                    if (mv.Count == 1)
                    {
                        breaker = true;
                        int val = mv[0];

                        if (isAutoSingle)
                        {
                            cellManager.FillCell(_y, _x, val);
                            return true;
                        }
                        else // 일반
                        {
                            //대사
                            string[] str = { "네이키드 싱글", $"이 셀에 {val} 말고는 들어갈 수 있는 값이 없습니다." };

                            //처방
                            var hc = MakeHC(null, objects[_y, _x]);
                            var toFill = MakeTuple((_y, _x), val);

                            hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                            return true;

                        }
                    }
                }
            }
        }
        return false;
    }

    public bool FindHiddenSingle(bool isAutoSingle = false) //히든 싱글
    {
        //row 검사
        for (int val = 1; val <= 9; val++)
        {
            for (int _y = 0; _y < 9; _y++)
            {
                var ev = sudokuController.GetMemoRow(_y, val);
                if (ev.Sum() == 1)
                {
                    breaker = true;
                    int _x = Array.IndexOf(ev, 1);

                    if (isAutoSingle)
                    {
                        cellManager.FillCell(_y, _x, val);
                        return true;
                    }
                    else // 일반
                    {
                        //대사
                        string[] str = { "히든 싱글", $"가로 행에서 이 셀에 들어갈 수 있는 값은 {val} 하나입니다." };

                        //처방
                        var hc = MakeHC(null, objects[_y, _x]);
                        var toFill = MakeTuple((_y, _x), val);

                        hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                        return true;

                    }
                }
            }
        }

        //col 검사
        for (int val = 1; val <= 9; val++)
        {
            for (int _x = 0; _x < 9; _x++)
            {
                var ev = sudokuController.GetMemoCol(_x, val);
                if (ev.Sum() == 1)
                {
                    breaker = true;
                    int _y = Array.IndexOf(ev, 1);

                    if (isAutoSingle)
                    {
                        cellManager.FillCell(_y, _x, val);
                        return true;
                    }
                    else // 일반
                    {
                        //대사
                        string[] str = { "히든 싱글", $"세로 열에서 이 셀에 들어갈 수 있는 값은 {val} 하나입니다." };

                        //처방
                        var hc = MakeHC(null, objects[_y, _x]);
                        var toFill = MakeTuple((_y, _x), val);

                        hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                        return true;

                    }
                }
            }
        }

        //SG 검사
        for (int val = 1; val <= 9; val++)
        {
            for (int _y = 0; _y < 3; _y++)
            {
                for (int _x = 0; _x < 3; _x++)
                {
                    var ev = sudokuController.GetMemoSG(_y, _x, val);
                    if (ev.Sum() == 1)
                    {
                        breaker = true;

                        if (isAutoSingle)
                        {
                            cellManager.FillCell(_y, _x, val);
                            return true;
                        }
                        else // 일반
                        {
                            //대사
                            string[] str = { "히든 싱글", $"3X3 서브그리드에서 이 셀에 들어갈 수 있는 값은 {val} 하나입니다." };

                            //처방
                            var hc = MakeHC(null, objects[_y, _x]);
                            var toFill = MakeTuple((_y, _x), val);

                            hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                            return true;

                        }
                    }
                }
            }
        }

        return false;
    }

    private void FindCrossPointing()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var evs = sudokuController.GetEmptyValueInSG(y, x);
                foreach (var ev in evs)
                {
                    var (rows, cols) = sudokuController.GetLinesDisabledBySG(y, x, ev);
                    //rows
                    if (rows.Count == 1)
                    {
                        int r = rows[0];

                        //교차점
                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();
                        for (int _x = 0; _x < 9; _x++)
                        {
                            if (_x >= x * 3 && _x < x * 3 + 3) //교차점 영역이 아닐 시
                            {
                                if (sudokuController.IsInMemoCell(r, _x, ev))
                                {
                                    hc.Add(memoObjects[r, _x, (ev - 1) / 3, (ev - 1) % 3]);
                                }
                                continue;
                            }

                            if (sudokuController.IsInMemoCell(r, _x, ev) == true) //교차점 영역일 시 
                            {
                                dc.Add(memoObjects[r, _x, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }

                        if (dc.Count != 0)
                        {
                            breaker = true;

                            //대사
                            string[] str = {
                                "인터섹션", $"{r + 1}행은 (서브그리드 조건을 충족하기 위해)강조된 셀들 중 하나에 무조건 {ev} 값이 들어가야 합니다",
                                $"따라서 이 셀들에는 {ev} 값이 들어갈 수 없습니다."
                            };

                            //처방
                            var hcList = MakeHCList(null, hc, dc);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);
                            return;
                        }
                    }

                    //cols
                    if (cols.Count == 1)
                    {
                        int c = cols[0];

                        //교차점
                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();
                        for (int _y = 0; _y < 9; _y++)
                        {
                            if (_y >= y * 3 && _y < y * 3 + 3) //교차점 영역이 아닐 시
                            {
                                if (sudokuController.IsInMemoCell(_y, c, ev))
                                {
                                    hc.Add(memoObjects[_y, c, (ev - 1) / 3, (ev - 1) % 3]);
                                }
                                continue;
                            }

                            if (sudokuController.IsInMemoCell(_y, c, ev) == true) //교차점 영역일 시 
                            {
                                dc.Add(memoObjects[_y, c, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }

                        if (dc.Count != 0)
                        {
                            breaker = true;

                            //대사
                            string[] str = {
                                "인터섹션", $"{c + 1}열은 (서브그리드 조건을 충족하기 위해)강조된 셀들 중 하나에 무조건 {ev} 값이 들어가야 합니다",
                                $"따라서 이 셀들에는 {ev} 값이 들어갈 수 없습니다."
                            };

                            //처방
                            var hcList = MakeHCList(null, hc, dc);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);
                            return;
                        }
                    }
                }
            }
        }
    }//인터섹션

    private void FindCrossClaiming()
    {
        //row
        for (int y = 0; y < 9; y++)
        {
            var evs = sudokuController.GetEmptyValueInRow(y);
            foreach (var ev in evs)
            {
                var SGs = sudokuController.GetSGsDisbledByRow(y, ev);
                //rows
                if (SGs.Count == 1)
                {
                    var sg = SGs[0];
                    //교차점
                    List<GameObject> hc = new List<GameObject>();
                    List<GameObject> dc = new List<GameObject>();

                    for (int _y = sg.Item1 * 3; _y < sg.Item1 * 3 + 3; _y++)
                    {
                        for (int _x = sg.Item2 * 3; _x < sg.Item2 * 3 + 3; _x++)
                        {
                            if (_y == y) //교차점 영역이 아닐 시
                            {
                                if (sudokuController.IsInMemoCell(_y, _x, ev))
                                {
                                    hc.Add(memoObjects[_y, _x, (ev - 1) / 3, (ev - 1) % 3]);
                                }
                                continue;
                            }

                            if (sudokuController.IsInMemoCell(_y, _x, ev) == true) //교차점 영역일 시 
                            {
                                dc.Add(memoObjects[_y, _x, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }
                    }

                    if (dc.Count != 0)
                    {
                        breaker = true;

                        //대사
                        string[] str = {
                                "인터섹션", $"{sg.Item1+1}-{sg.Item1+1} 서브그리드는 (행 조건을 충족하기 위해)강조된 셀들 중 하나에 무조건 {ev} 값이 들어가야 합니다",
                                $"따라서 이 셀들에는 {ev} 값이 들어갈 수 없습니다."
                            };

                        //처방
                        var hcList = MakeHCList(null, hc, dc);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);
                        return;
                    }
                }
            }
        }

        //col
        for (int x = 0; x < 9; x++)
        {
            var evs = sudokuController.GetEmptyValueInCol(x);
            foreach (var ev in evs)
            {
                var SGs = sudokuController.GetSGsDisbledByCol(x, ev);
                //rows
                if (SGs.Count == 1)
                {
                    var sg = SGs[0];
                    List<GameObject> hc = new List<GameObject>();
                    List<GameObject> dc = new List<GameObject>();

                    for (int _y = sg.Item1 * 3; _y < sg.Item1 * 3 + 3; _y++)
                    {
                        for (int _x = sg.Item2 * 3; _x < sg.Item2 * 3 + 3; _x++)
                        {
                            if (_x == x) //교차점 영역이 아닐 시
                            {
                                if (sudokuController.IsInMemoCell(_y, _x, ev))
                                {
                                    hc.Add(memoObjects[_y, _x, (ev - 1) / 3, (ev - 1) % 3]);
                                }
                                continue;
                            }

                            if (sudokuController.IsInMemoCell(_y, _x, ev) == true) //교차점 영역일 시 
                            {
                                dc.Add(memoObjects[_y, _x, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }
                    }

                    if (dc.Count != 0)
                    {
                        breaker = true;

                        //대사
                        string[] str = {
                                "인터섹션", $"{sg.Item1+1}-{sg.Item1+1} 서브그리드는 (행 조건을 충족하기 위해)강조된 셀들 중 하나에 무조건 {ev} 값이 들어가야 합니다",
                                $"따라서 이 셀들에는 {ev} 값이 들어갈 수 없습니다."
                            };

                        //처방
                        var hcList = MakeHCList(null, hc, dc);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);
                        return;
                    }
                }
            }
        }
    }

    private void FindNakedPair()
    {
        //row
        for (int y = 0; y < 9; y++)
        {
            var emptyXList = sudokuController.GetEmptyCellsInRow(y); //비어 있는 x좌표
            var mvList = sudokuController.GetMemoValuesInRow(y); //메모 안에 들어있는 값들의 모음
            for (int _x1 = 0; _x1 < emptyXList.Count - 1; _x1++)
            {
                for (int _x2 = _x1 + 1; _x2 < emptyXList.Count; _x2++)
                {
                    if (mvList[_x1].Count != 2 || mvList[_x2].Count != 2)
                    {
                        continue;
                    }

                    if (sudokuController.IsEqualMemoCell((y, emptyXList[_x1]), (y, emptyXList[_x2]))) // 네이키드 페어 발견
                    {
                        var mvs = mvList[_x1]; // 선택된 셀들 안에 들어있는 메모값들

                        // 선택된 셀들 안에 들어있는 메모값들
                        List<GameObject> dc = new List<GameObject>();

                        foreach (var emptyX in emptyXList) // 네이키드 페어 외의 나머지 셀들 조사
                        {
                            if (emptyX == emptyXList[_x1] || emptyX == emptyXList[_x2])
                            {
                                continue;
                            }

                            foreach (var mv in mvs)
                            {
                                if (sudokuController.IsInMemoCell(y, emptyX, mv))
                                {
                                    dc.Add(memoObjects[y, emptyX, (mv - 1) / 3, (mv - 1) % 3]);
                                }
                            }
                        }

                        if (dc.Count != 0)
                        {
                            breaker = true;

                            List<GameObject> hc = new List<GameObject>();

                            foreach (var mv in mvs)
                            {
                                hc.Add(objects[y, emptyXList[_x1]]);
                                hc.Add(objects[y, emptyXList[_x2]]);
                            }

                            //대사
                            string[] str = {
                                "네이키드 페어",
                            $"{y+1} 행의 강조된 두 셀에 값 {mvs[0]}, {mvs[1]}으로 이루어진 똑같은 구성의 메모 셀들입니다.",
                            $"이는 값 {mvs[0]}, {mvs[1]}이 이 행의 강조된 두 셀에서만 존재해야만 한다는 것을 의미합니다.",
                            "따라서 다음 메모 셀들을 삭제합니다."};

                            //처방
                            var hcList = MakeHCList(null, hc, hc, dc);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);

                            return;
                        }
                    }
                }
            }
        }

        //col
        for (int x = 0; x < 9; x++)
        {
            var emptyYList = sudokuController.GetEmptyCellsInCol(x); //비어 있는 y좌표
            var mvList = sudokuController.GetMemoValuesInCol(x); //메모 안에 들어있는 값들의 모음
            for (int _y1 = 0; _y1 < emptyYList.Count - 1; _y1++)
            {
                for (int _y2 = _y1 + 1; _y2 < emptyYList.Count; _y2++)
                {
                    if (mvList[_y1].Count != 2 || mvList[_y2].Count != 2)
                    {
                        continue;
                    }

                    if (sudokuController.IsEqualMemoCell((emptyYList[_y1], x), (emptyYList[_y2], x)))
                    { // 네이키드 페어 발견                    {
                        var mvs = mvList[_y1]; // 선택된 셀들 안에 들어있는 메모값들

                        // 선택된 셀들 안에 들어있는 메모값들
                        List<GameObject> dc = new List<GameObject>();

                        foreach (var emptyY in emptyYList) // 네이키드 페어 외의 나머지 셀들 조사
                        {
                            if (emptyY == emptyYList[_y1] || emptyY == emptyYList[_y2])
                            {
                                continue;
                            }

                            foreach (var mv in mvs)
                            {
                                if (sudokuController.IsInMemoCell(emptyY, x, mv))
                                {
                                    dc.Add(memoObjects[emptyY, x, (mv - 1) / 3, (mv - 1) % 3]);
                                }
                            }
                        }

                        if (dc.Count != 0)
                        {
                            breaker = true;

                            List<GameObject> hc = new List<GameObject>();

                            foreach (var mv in mvs)
                            {
                                hc.Add(memoObjects[emptyYList[_y1], x, (mv - 1) / 3, (mv - 1) % 3]);
                                hc.Add(memoObjects[emptyYList[_y2], x, (mv - 1) / 3, (mv - 1) % 3]);
                            }

                            //대사
                            string[] str = {
                                "네이키드 페어",
                            $"{x+1} 열의 강조된 두 셀에 값 {mvs[0]}, {mvs[1]}으로 이루어진 똑같은 구성의 메모 셀들입니다.",
                            $"이는 값 {mvs[0]}, {mvs[1]}이 이 열의 강조된 두 셀에서만 존재해야만 한다는 것을 의미합니다.",
                            "따라서 다음 메모 셀들을 삭제합니다."};

                            //처방
                            var hcList = MakeHCList(null, hc, hc, dc);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);

                            return;
                        }
                    }
                }
            }
        }

        //SG
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var emptyYXList = sudokuController.GetEmptyCellsInSG(y, x); //비어 있는 y좌표
                var mvList = sudokuController.GetMemoValuesInSG(y, x); //메모 안에 들어있는 값들의 모음
                for (int _sg1 = 0; _sg1 < emptyYXList.Count - 1; _sg1++)
                {
                    for (int _sg2 = _sg1 + 1; _sg2 < emptyYXList.Count; _sg2++)
                    {
                        if (mvList[_sg1].Count != 2 || mvList[_sg2].Count != 2)
                        {
                            continue;
                        }

                        if (sudokuController.IsEqualMemoCell(
                            (emptyYXList[_sg1].Item1, emptyYXList[_sg1].Item2),
                            (emptyYXList[_sg2].Item1, emptyYXList[_sg2].Item2))) // 네이키드 페어 발견
                        {
                            var mvs = mvList[_sg1]; // 선택된 셀들 안에 들어있는 메모값들

                            // 선택된 셀들 안에 들어있는 메모값들
                            List<GameObject> dc = new List<GameObject>();

                            foreach (var emptyYX in emptyYXList) // 네이키드 페어 외의 나머지 셀들 조사
                            {
                                if ((emptyYX.Item1 == emptyYXList[_sg1].Item1) && (emptyYX.Item2 == emptyYXList[_sg1].Item2) ||
                                    (emptyYX.Item1 == emptyYXList[_sg2].Item1) && (emptyYX.Item2 == emptyYXList[_sg2].Item2))
                                {
                                    continue;
                                }

                                foreach (var mv in mvs)
                                {
                                    if (sudokuController.IsInMemoCell(emptyYX.Item1, emptyYX.Item2, mv))
                                    {
                                        dc.Add(memoObjects[emptyYX.Item1, emptyYX.Item2, (mv - 1) / 3, (mv - 1) % 3]);
                                    }
                                }
                            }

                            if (dc.Count != 0)
                            {
                                breaker = true;

                                List<GameObject> hc = new List<GameObject>();

                                foreach (var mv in mvs)
                                {
                                    hc.Add(memoObjects[emptyYXList[_sg1].Item1, emptyYXList[_sg1].Item2, (mv - 1) / 3, (mv - 1) % 3]);
                                    hc.Add(memoObjects[emptyYXList[_sg2].Item1, emptyYXList[_sg2].Item2, (mv - 1) / 3, (mv - 1) % 3]);
                                }

                                //대사
                                string[] str = {
                                "네이키드 페어",
                            $"{y*3+x+1} 번째 서브그리드의 강조된 두 셀은 값 {mvs[0]}, {mvs[1]}으로 이루어진 똑같은 구성의 메모 셀들입니다.",
                            $"이는 값 {mvs[0]}, {mvs[1]}이 이 열의 강조된 두 셀에서만 존재해야만 한다는 것을 의미합니다.",
                            "따라서 다음 메모 셀들을 삭제합니다."};

                                //처방
                                var hcList = MakeHCList(null, hc, hc, dc);
                                hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);

                                return;
                            }
                        }
                    }
                }
            }
        }
    } 

    private void FindXWing()
    {
        //row
        for (int y1 = 0; y1 < 8; y1++)
        {
            for (int y2 = y1 + 1; y2 < 9; y2++)
            {
                for (int val = 1; val <= 9; val++)
                {
                    var mcr1 = sudokuController.GetMemoCellInRow(y1, val);
                    var mcr2 = sudokuController.GetMemoCellInRow(y2, val);

                    if (mcr1.Count != 2 || mcr2.Count != 2)
                    {
                        continue;
                    }

                    if (mcr1[0] != mcr2[0] || mcr1[1] != mcr2[1])
                    {
                        continue;
                    }

                    //Xwing 발견
                    var mcc1 = sudokuController.GetMemoCellInCol(mcr1[0], val);
                    var mcc2 = sudokuController.GetMemoCellInCol(mcr1[1], val);


                    if (mcc1.Count == 2 && mcc2.Count == 2)
                    {
                        continue;
                    }
                    //조건 충족
                    breaker = true;
                    List<GameObject> hc = new List<GameObject>();
                    List<GameObject> dc = new List<GameObject>();
                    List<GameObject> hdc = new List<GameObject>();

                    //강조
                    for (int i = 0; i < 2; i++)
                    {
                        hc.Add(objects[y1, mcr1[i]]);
                        hc.Add(objects[y2, mcr1[i]]);
                    }

                    //삭제
                    foreach (var mc in mcc1)
                    {
                        if (mc == y1 || mc == y2)
                        {
                            continue;
                        }
                        dc.Add(memoObjects[mc, mcr1[0], sudokuController.ValToY(val), sudokuController.ValToX(val)]);
                        hdc.Add(objects[mc, mcr1[0]]);
                    }

                    foreach (var mc in mcc2)
                    {
                        if (mc == y1 || mc == y2)
                        {
                            continue;
                        }
                        dc.Add(memoObjects[mc, mcr1[1], sudokuController.ValToY(val), sudokuController.ValToX(val)]);
                        hdc.Add(objects[mc, mcr1[1]]);
                    }


                    //대사
                    string[] str = {
                    "X-윙",
                    $"{y1+1}행과 {y2+1}행에는 각각 같은 열에 두 셀씩 {val}이 들어갈 수 있습니다.",
                    $"이렇게 강조된 네 셀 안에만 {val} 값이 들어갈 수 있습니다.",
                    $"따라서 다음 셀들엔 {val} 값이 들어갈 수 없습니다."
                };

                    //처방
                    var hcList = MakeHCList(null, hc, hc, hdc);
                    hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);

                    return;
                }
            }
        }

        //col
        for (int x1 = 0; x1 < 8; x1++)
        {
            for (int x2 = x1 + 1; x2 < 9; x2++)
            {
                for (int val = 1; val <= 9; val++)
                {
                    var mcc1 = sudokuController.GetMemoCellInCol(x1, val);
                    var mcc2 = sudokuController.GetMemoCellInCol(x2, val);

                    if (mcc1.Count != 2 || mcc2.Count != 2)
                    {
                        continue;
                    }
                    if (mcc1[0] != mcc2[0] || mcc1[1] != mcc2[1])
                    {
                        continue;
                    }

                    //Xwing 발견
                    var mcr1 = sudokuController.GetMemoCellInRow(mcc1[0], val);
                    var mcr2 = sudokuController.GetMemoCellInRow(mcc1[1], val);

                    if (mcr1.Count == 2 && mcr2.Count == 2)
                    {
                        continue;
                    }

                    //조건 충족
                    breaker = true;
                    List<GameObject> hc = new List<GameObject>();
                    List<GameObject> dc = new List<GameObject>();
                    List<GameObject> hdc = new List<GameObject>();

                    //강조
                    for (int i = 0; i < 2; i++)
                    {
                        hc.Add(objects[mcc1[i], x1]);
                        hc.Add(objects[mcc1[i], x2]);
                    }

                    //삭제
                    foreach (var mc in mcr1)
                    {
                        if (mc == x1 || mc == x2)
                        {
                            continue;
                        }
                        dc.Add(memoObjects[mcc1[0], mc, sudokuController.ValToY(val), sudokuController.ValToX(val)]);
                        hdc.Add(objects[mcc1[0], mc]);
                    }

                    foreach (var mc in mcr2)
                    {
                        if (mc == x1 || mc == x2)
                        {
                            continue;
                        }
                        dc.Add(memoObjects[mcc1[1], mc, sudokuController.ValToY(val), sudokuController.ValToX(val)]);
                        hdc.Add(objects[mcc1[1], mc]);
                    }


                    //대사
                    string[] str = {
                    "X-윙",
                    $"{x1+1}열과 {x2+1}열에는 각각 같은 행에 두 셀씩 {val}이 들어갈 수 있습니다.",
                    $"이렇게 강조된 네 셀 안에만 {val} 값이 들어갈 수 있습니다.",
                    $"따라서 다음 셀들엔 {val} 값이 들어갈 수 없습니다."
                };

                    //처방
                    var hcList = MakeHCList(null, hc, hc, hdc);
                    hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);

                    return;
                }
            }
        }
    }
    
    private void FindFromFullSudoku()
    {
        for (int _y = 0; _y < 9; _y++)
        {
            for (int _x = 0; _x < 9; _x++)
            {
                if (SudokuManager.sudoku[_y, _x] == 0)
                {
                    breaker = true;
                    int val = SudokuManager.fullSudoku[_y, _x];

                    //대사
                    string[] str = { "길라잡이", $"{_y + 1}행 {_x + 1}열의 값은 {val} 입니다." };

                    //처방
                    var hc = MakeHC(null, objects[_y, _x]);
                    var toFill = MakeTuple((_y, _x), val);
                    hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                }
            }
        }
    }

    private List<GameObject> MakeHC(params GameObject[] objs)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (var obj in objs)
        {
            list.Add(obj);
        }
        return list;
    }

    private List<List<GameObject>> MakeHCList(params List<GameObject>[] objs)
    {
        List<List<GameObject>> list = new List<List<GameObject>>();
        foreach (var obj in objs)
        {
            list.Add(obj);
        }
        return list;
    }

    private Tuple<(int, int), int> MakeTuple((int, int) YX, int value)
    {
        Tuple<(int, int), int> tuple = new Tuple<(int, int), int>(YX, value);
        return tuple;
    }
}
