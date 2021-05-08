using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : SudokuController
{
    public SudokuController sudokuController;
    public HintDialogManager hintDialogManager;
    public AutoMemoManager autoMemoManager;

    public GameObject[,] objects;
    public GameObject[,,,] memoObjects;

    private bool breaker = false;

    protected override void Start()
    {
        base.Start();

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
            if (!IsNormalSudoku())
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

        FindIntersectPointing();
        if (breaker)
        {
            return;
        }

        FindIntersectClaiming();
        if (breaker)
        {
            return;
        }

        FindNakedPair();
        if (breaker)
        {
            return;
        }

        FindNakedTriple();
        if (breaker)
        {
            return;
        }

        FindHiddenPair();
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
        var p1 = CompareWithFullSudoku();
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
        var p2 = CompareMemoWithFullSudoku();
        if (p2.Count != 0)
        {
            breaker = true;

            //대사
            string[] str = { "메모가 불충분합니다.", "메모를 수정합니다." };
            hintDialogManager.StartDialog(str);

            //처방
            autoMemoManager.RunAutoMemo(false);
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
            var ev = GetEmptyValueInRow(_y);
            if (ev.Count == 1)
            {
                int _x = GetEmptyCellsInRow(_y)[0];
                int val = ev[0];
                breaker = true;

                //대사
                string[] str = { "풀 하우스", $"가로 행에 한 값 {ev[0]}만 비어 있습니다." };

                //처방
                var hc = MakeHC(null, objects[_y, _x]);
                var hb = MakeBundle(_y);
                var hbl = MakeBundleList(null, hb);
                var toFill = MakeTuple((_y, _x), val);

                hintDialogManager.StartDialogAndFillCell(str, hc, toFill, hbl);
                return;
            }
        }

        //col 검사
        for (int _x = 0; _x < 9; _x++)
        {
            var ev = GetEmptyValueInCol(_x);
            if (ev.Count == 1)
            {
                int _y = GetEmptyCellsInCol(_x)[0];
                int val = ev[0];
                breaker = true;

                //대사
                string[] str = { "풀 하우스", $"세로 열에 한 값 {ev[0]}만 비어 있습니다." };

                //처방
                var hc = MakeHC(null, objects[_y, _x]);
                var hb = MakeBundle(9 + _x);
                var hbl = MakeBundleList(null, hb);
                var toFill = MakeTuple((_y, _x), val);

                hintDialogManager.StartDialogAndFillCell(str, hc, toFill, hbl);
                return;
            }
        }

        //서브그리드
        for (int _y = 0; _y < 3; _y++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                var ev = GetEmptyValueInSG(_y, _x);
                if (ev.Count == 1)
                {
                    var cell = GetEmptyCellsInSG(_y, _x)[0];
                    int val = ev[0];
                    breaker = true;

                    //대사
                    string[] str = { "풀 하우스", $"서브그리드에 한 값 {val}만 비어 있습니다." };

                    //처방
                    var hc = MakeHC(null, objects[cell.Item1, cell.Item2]);
                    var hb = MakeBundle(18 + YXToVal(_y, _x));
                    var hbl = MakeBundleList(null, hb);
                    var toFill = MakeTuple((_y, _x), val);

                    hintDialogManager.StartDialogAndFillCell(str, hc, toFill, hbl);
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
                if (IsEmptyCell(_y, _x))
                {
                    var mv = GetActiveMemoValue(_y, _x);

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
        for (int _y = 0; _y < 9; _y++)
        {
            for (int val = 1; val <= 9; val++)
            {
                var ev = GetMemoRow(_y, val);
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
                        string[] str = { "히든 싱글", $"가로 행에서 {val}은 이 셀에만 들어갈 수 있습니다." };

                        //처방
                        var hc = MakeHC(null, objects[_y, _x]);
                        var hb = MakeBundle(_y);
                        var hbl = MakeBundleList(null, hb);
                        var toFill = MakeTuple((_y, _x), val);

                        hintDialogManager.StartDialogAndFillCell(str, hc, toFill, hbl);
                        return true;

                    }
                }
            }
        }

        //col 검사
        for (int _x = 0; _x < 9; _x++)
        {
            for (int val = 1; val <= 9; val++)
            {
                var ev = GetMemoCol(_x, val);
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
                        string[] str = { "히든 싱글", $"세로 열에서 {val}은 이 셀에만 들어갈 수 있습니다." };

                        //처방
                        var hc = MakeHC(null, objects[_y, _x]);
                        var hb = MakeBundle(9 + _x);
                        var hbl = MakeBundleList(null, hb);
                        var toFill = MakeTuple((_y, _x), val);

                        hintDialogManager.StartDialogAndFillCell(str, hc, toFill, hbl);
                        return true;

                    }
                }
            }
        }

        //SG 검사
        for (int _y = 0; _y < 3; _y++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                for (int val = 1; val <= 9; val++)
                {
                    var ev = GetMemoSG(_y, _x, val);
                    if (ev.Sum() == 1)
                    {
                        breaker = true;
                        int sg = Array.IndexOf(ev, 1);
                        int sgy = sg / 3;
                        int sgx = sg % 3;

                        if (isAutoSingle)
                        {
                            cellManager.FillCell(_y * 3 + sgy, _x * 3 + sgx, val);
                            return true;
                        }
                        else // 일반
                        {
                            //대사
                            string[] str = { "히든 싱글", $"3X3 서브그리드에서 {val}은 이 셀에만 들어갈 수 있습니다." };

                            //처방
                            var hc = MakeHC(null, objects[_y * 3 + sgy, _x * 3 + sgx]);
                            var hb = MakeBundle(18 + _y * 3 + sgy + _x * 3 + sgx);
                            var hbl = MakeBundleList(null, hb);
                            var toFill = MakeTuple((_y * 3 + sgy, _x * 3 + sgx), val);

                            hintDialogManager.StartDialogAndFillCell(str, hc, toFill, hbl);
                            return true;

                        }
                    }
                }
            }
        }

        return false;
    }

    private void FindIntersectPointing()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var evs = GetEmptyValueInSG(y, x);
                foreach (var ev in evs)
                {
                    var (rows, cols) = GetLinesDisabledBySG(y, x, ev);
                    //rows
                    if (rows.Count == 1)
                    {
                        int r = rows[0];

                        //교차점
                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();
                        List<GameObject> hdc = new List<GameObject>();
                        for (int _x = 0; _x < 9; _x++)
                        {
                            if (_x >= x * 3 && _x < x * 3 + 3) //교차점 영역이 아닐 시
                            {
                                if (IsInMemoCell(r, _x, ev))
                                {
                                    hc.Add(objects[r, _x]);
                                }
                                continue;
                            }

                            if (IsInMemoCell(r, _x, ev) == true) //교차점 영역일 시 
                            {
                                hdc.Add(objects[r, _x]);
                                dc.Add(memoObjects[r, _x, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }

                        if (dc.Count != 0)
                        {
                            breaker = true;

                            //대사
                            string[] str = {
                                "인터섹션", $"{r + 1}행은 서브그리드 조건을 충족하기 위해 강조된 셀들 중 하나에 무조건 {ev} 값이 들어가야 합니다.",
                                $"따라서 이 셀들에는 {ev} 값이 들어갈 수 없습니다."
                            };

                            //처방
                            var hcList = MakeHCList(null, hc, hdc);
                            var hb = MakeBundle(r, 18 + YXToVal(y, x));
                            var hbl = MakeBundleList(null, hb, null);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hbl);
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
                        List<GameObject> hdc = new List<GameObject>();
                        for (int _y = 0; _y < 9; _y++)
                        {
                            if (_y >= y * 3 && _y < y * 3 + 3) //교차점 영역이 아닐 시
                            {
                                if (IsInMemoCell(_y, c, ev))
                                {
                                    hc.Add(objects[_y, c]);
                                }
                                continue;
                            }

                            if (IsInMemoCell(_y, c, ev) == true) //교차점 영역일 시 
                            {
                                hdc.Add(objects[_y, c]);
                                dc.Add(memoObjects[_y, c, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }

                        if (dc.Count != 0)
                        {
                            breaker = true;

                            //대사
                            string[] str = {
                                "인터섹션", $"{c + 1}열은 서브그리드 조건을 충족하기 위해 강조된 셀들 중 하나에 무조건 {ev} 값이 들어가야 합니다.",
                                $"따라서 이 셀들에는 {ev} 값이 들어갈 수 없습니다."
                            };

                            //처방
                            var hcList = MakeHCList(null, hc, hdc);
                            var hb = MakeBundle(9 + c, 18 + YXToVal(y, x));
                            var hbl = MakeBundleList(null, hb, null);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hbl);
                            return;
                        }
                    }
                }
            }
        }
    }//인터섹션

    private void FindIntersectClaiming() //인터섹션
    {
        //row
        for (int y = 0; y < 9; y++)
        {
            var evs = GetEmptyValueInRow(y);
            foreach (var ev in evs)
            {
                var SGs = GetSGsDisbledByRow(y, ev);
                //rows
                if (SGs.Count == 1)
                {
                    var sg = SGs[0];
                    //교차점
                    List<GameObject> hc = new List<GameObject>();
                    List<GameObject> dc = new List<GameObject>();
                    List<GameObject> hdc = new List<GameObject>();

                    for (int _y = sg.Item1 * 3; _y < sg.Item1 * 3 + 3; _y++)
                    {
                        for (int _x = sg.Item2 * 3; _x < sg.Item2 * 3 + 3; _x++)
                        {
                            if (_y == y) //교차점 영역이 아닐 시
                            {
                                if (IsInMemoCell(_y, _x, ev))
                                {
                                    hc.Add(objects[_y, _x]);
                                }
                                continue;
                            }

                            if (IsInMemoCell(_y, _x, ev) == true) //교차점 영역일 시 
                            {
                                hdc.Add(objects[_y, _x]);
                                dc.Add(memoObjects[_y, _x, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }
                    }

                    if (dc.Count != 0)
                    {
                        breaker = true;

                        //대사
                        string[] str = {
                                "인터섹션", $"{sg.Item1+1}-{sg.Item2+1} 서브그리드는 행 조건을 충족하기 위해 강조된 셀들 중 하나에 무조건 {ev} 값이 들어가야 합니다.",
                                $"따라서 이 셀들에는 {ev} 값이 들어갈 수 없습니다."
                            };

                        //처방
                        var hcList = MakeHCList(null, hc, hdc);
                        var hb = MakeBundle(y, 18 + sg.Item1 * 3 + sg.Item2);
                        var hbl = MakeBundleList(null, hb, null);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hbl);
                        return;
                    }
                }
            }
        }

        //col
        for (int x = 0; x < 9; x++)
        {
            var evs = GetEmptyValueInCol(x);
            foreach (var ev in evs)
            {
                var SGs = GetSGsDisbledByCol(x, ev);
                //rows
                if (SGs.Count == 1)
                {
                    var sg = SGs[0];
                    List<GameObject> hc = new List<GameObject>();
                    List<GameObject> dc = new List<GameObject>();
                    List<GameObject> hdc = new List<GameObject>();

                    for (int _y = sg.Item1 * 3; _y < sg.Item1 * 3 + 3; _y++)
                    {
                        for (int _x = sg.Item2 * 3; _x < sg.Item2 * 3 + 3; _x++)
                        {
                            if (_x == x) //교차점 영역이 아닐 시
                            {
                                if (IsInMemoCell(_y, _x, ev))
                                {
                                    hc.Add(objects[_y, _x]);
                                }
                                continue;
                            }

                            if (IsInMemoCell(_y, _x, ev) == true) //교차점 영역일 시 
                            {
                                hdc.Add(objects[_y, _x]);
                                dc.Add(memoObjects[_y, _x, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }
                    }

                    if (dc.Count != 0)
                    {
                        breaker = true;

                        //대사
                        string[] str = {
                                "인터섹션", $"{sg.Item1+1}-{sg.Item2+1} 서브그리드는 열 조건을 충족하기 위해 강조된 셀들 중 하나에 무조건 {ev} 값이 들어가야 합니다.",
                                $"따라서 이 셀들에는 {ev} 값이 들어갈 수 없습니다."
                            };

                        //처방
                        var hcList = MakeHCList(null, hc, hdc);
                        var hb = MakeBundle(9 + x, 18 + sg.Item1 * 3 + sg.Item2);
                        var hbl = MakeBundleList(null, hb, null);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hbl);
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
            var emptyXList = GetEmptyCellsInRow(y); //비어 있는 x좌표
            var mvList = GetMemoValuesInRow(y); //메모 안에 들어있는 값들의 모음
            for (int _x1 = 0; _x1 < emptyXList.Count - 1; _x1++)
            {
                for (int _x2 = _x1 + 1; _x2 < emptyXList.Count; _x2++)
                {
                    if (mvList[_x1].Count != 2 || mvList[_x2].Count != 2)
                    {
                        continue;
                    }

                    if (IsEqualMemoCell((y, emptyXList[_x1]), (y, emptyXList[_x2]))) // 네이키드 페어 발견
                    {
                        var mvs = mvList[_x1]; // 선택된 셀들 안에 들어있는 메모값들

                        // 선택된 셀들 안에 들어있는 메모값들
                        List<GameObject> dc = new List<GameObject>();
                        List<GameObject> hdc = new List<GameObject>();

                        foreach (var emptyX in emptyXList) // 네이키드 페어 외의 나머지 셀들 조사
                        {
                            if (emptyX == emptyXList[_x1] || emptyX == emptyXList[_x2])
                            {
                                continue;
                            }

                            foreach (var mv in mvs)
                            {
                                if (IsInMemoCell(y, emptyX, mv))
                                {
                                    dc.Add(memoObjects[y, emptyX, (mv - 1) / 3, (mv - 1) % 3]);
                                    hdc.Add(objects[y, emptyX]);
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
                            var hb = MakeBundle(y);
                            var hbl = MakeBundleList(null, hb, hb, null);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hbl);

                            return;
                        }
                    }
                }
            }
        }

        //col
        for (int x = 0; x < 9; x++)
        {
            var emptyYList = GetEmptyCellsInCol(x); //비어 있는 y좌표
            var mvList = GetMemoValuesInCol(x); //메모 안에 들어있는 값들의 모음
            for (int _y1 = 0; _y1 < emptyYList.Count - 1; _y1++)
            {
                for (int _y2 = _y1 + 1; _y2 < emptyYList.Count; _y2++)
                {
                    if (mvList[_y1].Count != 2 || mvList[_y2].Count != 2)
                    {
                        continue;
                    }

                    if (IsEqualMemoCell((emptyYList[_y1], x), (emptyYList[_y2], x)))
                    { // 네이키드 페어 발견                    {
                        var mvs = mvList[_y1]; // 선택된 셀들 안에 들어있는 메모값들

                        // 선택된 셀들 안에 들어있는 메모값들
                        List<GameObject> dc = new List<GameObject>();
                        List<GameObject> hdc = new List<GameObject>();

                        foreach (var emptyY in emptyYList) // 네이키드 페어 외의 나머지 셀들 조사
                        {
                            if (emptyY == emptyYList[_y1] || emptyY == emptyYList[_y2])
                            {
                                continue;
                            }

                            foreach (var mv in mvs)
                            {
                                if (IsInMemoCell(emptyY, x, mv))
                                {
                                    dc.Add(memoObjects[emptyY, x, (mv - 1) / 3, (mv - 1) % 3]);
                                    hdc.Add(objects[emptyY, x]);
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
                            var hb = MakeBundle(9 + x);
                            var hbl = MakeBundleList(null, hb, hb, null);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hbl);

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
                var emptyYXList = GetEmptyCellsInSG(y, x); //비어 있는 y좌표
                var mvList = GetMemoValuesInSG(y, x); //메모 안에 들어있는 값들의 모음
                for (int _sg1 = 0; _sg1 < emptyYXList.Count - 1; _sg1++)
                {
                    for (int _sg2 = _sg1 + 1; _sg2 < emptyYXList.Count; _sg2++)
                    {
                        if (mvList[_sg1].Count != 2 || mvList[_sg2].Count != 2)
                        {
                            continue;
                        }

                        if (IsEqualMemoCell(
                            (emptyYXList[_sg1].Item1, emptyYXList[_sg1].Item2),
                            (emptyYXList[_sg2].Item1, emptyYXList[_sg2].Item2))) // 네이키드 페어 발견
                        {
                            var mvs = mvList[_sg1]; // 선택된 셀들 안에 들어있는 메모값들

                            // 선택된 셀들 안에 들어있는 메모값들
                            List<GameObject> dc = new List<GameObject>();
                            List<GameObject> hdc = new List<GameObject>();

                            foreach (var emptyYX in emptyYXList) // 네이키드 페어 외의 나머지 셀들 조사
                            {
                                if ((emptyYX.Item1 == emptyYXList[_sg1].Item1) && (emptyYX.Item2 == emptyYXList[_sg1].Item2) ||
                                    (emptyYX.Item1 == emptyYXList[_sg2].Item1) && (emptyYX.Item2 == emptyYXList[_sg2].Item2))
                                {
                                    continue;
                                }

                                foreach (var mv in mvs)
                                {
                                    if (IsInMemoCell(emptyYX.Item1, emptyYX.Item2, mv))
                                    {
                                        dc.Add(memoObjects[emptyYX.Item1, emptyYX.Item2, (mv - 1) / 3, (mv - 1) % 3]);
                                        hdc.Add(objects[emptyYX.Item1, emptyYX.Item2]);
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
                            $"{y+1}-{x+1} 서브그리드의 강조된 두 셀은 값 {mvs[0]}, {mvs[1]}으로 이루어진 똑같은 구성의 메모 셀들입니다.",
                            $"이는 값 {mvs[0]}, {mvs[1]}이 이 열의 강조된 두 셀에서만 존재해야만 한다는 것을 의미합니다.",
                            "따라서 다음 메모 셀들을 삭제합니다."};

                                //처방
                                var hcList = MakeHCList(null, hc, hc, dc);
                                var hb = MakeBundle(18 + y * 3 + x);
                                var hbl = MakeBundleList(null, hb, hb, null);
                                hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hbl);

                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    private void FindHiddenPair() //히든 페어
    {
        //row 검사
        for (int _y = 0; _y < 9; _y++)
        {
            var evr = GetEmptyValueInRow(_y);
            var evr_cnt = evr.Count;
            for (int _x1 = 0; _x1 < evr_cnt - 1; _x1++)
            {
                for (int _x2 = _x1 + 1; _x2 < evr_cnt; _x2++)
                {
                    var ev1_row = GetMemoCellInRow(_y, evr[_x1]); //x 좌표 리스트
                    var ev2_row = GetMemoCellInRow(_y, evr[_x2]);

                    if (ev1_row.Count != 2 || ev2_row.Count != 2)
                    {
                        continue;
                    }

                    if (ev1_row[0] != ev2_row[0] || ev1_row[1] != ev2_row[1])
                    {
                        continue;
                    }

                    int[] pair = { evr[_x1], evr[_x2] };
                    //페어 값: (evr[_x1], evr[_x2]) 좌표 : (ev1_row[0], ev1_row[1])

                    List<GameObject> hc = new List<GameObject>();
                    List<GameObject> dc = new List<GameObject>();

                    foreach (var ev in ev1_row) // x 좌표
                    {
                        var mvs = GetActiveMemoValue(_y, ev);
                        foreach (var mv in mvs) // 셀의 모든 메모값 
                        {
                            if (mv == pair[0])
                            {
                                hc.Add(memoObjects[_y, ev, ValToY(mv), ValToX(mv)]);
                            }
                            else if (mv == pair[1])
                            {
                                hc.Add(memoObjects[_y, ev, ValToY(mv), ValToX(mv)]);
                            }
                            else
                            {
                                dc.Add(memoObjects[_y, ev, ValToY(mv), ValToX(mv)]);
                            }
                        }
                    }

                    if (dc.Count != 0)
                    {
                        breaker = true;

                        //대사
                        string[] str = { "히든 페어",
                            $"{_y + 1}행에서 {pair[0]} 값과 {pair[1]} 값이 이 두 셀에만 존재합니다.",
                            $"즉, 이 두 셀에는 {pair[0]} 값과 {pair[1]} 값만 들어갈 수 있습니다.",
                            "따라서 다른 값들은 이 셀에 들어갈 수 없습니다."};

                        //처방
                        var hclist = MakeHCList(null, hc, hc, dc);
                        var hb = MakeBundle(_y);
                        var hbl = MakeBundleList(null, hb, hb, null);

                        hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hbl);

                        return;
                    }
                }
            }
        }

        //col 검사
        for (int _x = 0; _x < 9; _x++)
        {
            var evc = GetEmptyValueInCol(_x);
            var evc_cnt = evc.Count;
            for (int _y1 = 0; _y1 < evc_cnt - 1; _y1++)
            {
                for (int _y2 = _y1 + 1; _y2 < evc_cnt; _y2++)
                {
                    var ev1_col = GetMemoCellInCol(_x, evc[_y1]); //y 좌표 리스트
                    var ev2_col = GetMemoCellInCol(_x, evc[_y2]);

                    if (ev1_col.Count != 2 || ev2_col.Count != 2)
                    {
                        continue;
                    }

                    if (ev1_col[0] != ev2_col[0] || ev1_col[1] != ev2_col[1])
                    {
                        continue;
                    }

                    int[] pair = { evc[_y1], evc[_y2] };
                    //페어 값: (evx[_y1], evc[_y2]) 좌표 : (ev1_col[0], ev1_col[1])

                    List<GameObject> hc = new List<GameObject>();
                    List<GameObject> dc = new List<GameObject>();

                    foreach (var ev in ev1_col) // y 좌표
                    {
                        var mvs = GetActiveMemoValue(ev, _x);
                        foreach (var mv in mvs) // 셀의 모든 메모값 
                        {
                            if (mv == pair[0])
                            {
                                hc.Add(memoObjects[ev, _x, ValToY(mv), ValToX(mv)]);
                            }
                            else if (mv == pair[1])
                            {
                                hc.Add(memoObjects[ev, _x, ValToY(mv), ValToX(mv)]);
                            }
                            else
                            {
                                dc.Add(memoObjects[ev, _x, ValToY(mv), ValToX(mv)]);
                            }
                        }
                    }

                    if (dc.Count != 0)
                    {
                        breaker = true;

                        //대사
                        string[] str = { "히든 페어",
                            $"{_x + 1}열에서 {pair[0]} 값과 {pair[1]} 값이 이 두 셀에만 존재합니다.",
                            $"즉, 이 두 셀에는 {pair[0]} 값과 {pair[1]} 값만 들어갈 수 있습니다.",
                            "따라서 다른 값들은 이 셀에 들어갈 수 없습니다."};

                        //처방
                        var hclist = MakeHCList(null, hc, hc, dc);
                        var hb = MakeBundle(9 + _x);
                        var hbl = MakeBundleList(null, hb, hb, null);

                        hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hbl);

                        return;
                    }
                }
            }
        }

        //sg 검사
        for (int _y = 0; _y < 3; _y++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                var evsg = GetEmptyValueInSG(_y, _x);
                var evsg_cnt = evsg.Count;
                for (int _sg1 = 0; _sg1 < evsg_cnt - 1; _sg1++)
                {
                    for (int _sg2 = _sg1 + 1; _sg2 < evsg_cnt; _sg2++)
                    {
                        var ev1_sg = GetMemoCellInSG(_y, _x, evsg[_sg1]); //sg 좌표 리스트
                        var ev2_sg = GetMemoCellInSG(_y, _x, evsg[_sg2]);
                        if (ev1_sg.Count != 2 || ev2_sg.Count != 2)
                        {
                            continue;
                        }

                        if (ev1_sg[0] != ev2_sg[0] || ev1_sg[1] != ev2_sg[1])
                        {
                            continue;
                        }

                        int[] pair = { evsg[_sg1], evsg[_sg2] };
                        //페어 값: (evsg[_sg1], evsg[_sg2]) 좌표 : (ev1_sg[0], ev1_sg[1])

                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();

                        foreach (var ev in ev1_sg) // y 좌표
                        {
                            var mvs = GetActiveMemoValue(ev.Item1, ev.Item2);
                            foreach (var mv in mvs) // 셀의 모든 메모값 
                            {
                                if (mv == pair[0])
                                {
                                    hc.Add(memoObjects[ev.Item1, ev.Item2, ValToY(mv), ValToX(mv)]);
                                }
                                else if (mv == pair[1])
                                {
                                    hc.Add(memoObjects[ev.Item1, ev.Item2, ValToY(mv), ValToX(mv)]);
                                }
                                else
                                {
                                    dc.Add(memoObjects[ev.Item1, ev.Item2, ValToY(mv), ValToX(mv)]);
                                }
                            }
                        }

                        if (dc.Count != 0)
                        {
                            breaker = true;

                            //대사
                            string[] str = { "히든 페어",
                            $"{_y+1}-{_x+1}서브그리드에서 {pair[0]} 값과 {pair[1]} 값이 이 두 셀에만 존재합니다.",
                            $"즉, 이 두 셀에는 {pair[0]} 값과 {pair[1]} 값만 들어갈 수 있습니다.",
                            "따라서 다른 값들은 이 셀에 들어갈 수 없습니다."};

                            //처방
                            var hclist = MakeHCList(null, hc, hc, dc);
                            var hb = MakeBundle(18 + _y * 3 + _x);
                            var hbl = MakeBundleList(null, hb, hb, null);

                            hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hbl);

                            return;
                        }
                    }
                }
            }
        }
    }

    private void FindNakedTriple()
    {
        //row
        for (int y = 0; y < 9; y++)
        {
            var ec_row = GetEmptyCellsInRow(y); //빈 좌표 모음
            var ec_count = ec_row.Count;
            if (ec_count < 5)
            {
                continue;
            }

            for (int x1 = 0; x1 < ec_count - 2; x1++)
            {
                for (int x2 = x1 + 1; x2 < ec_count - 1; x2++)
                {
                    for (int x3 = x2 + 1; x3 < ec_count; x3++)
                    {
                        List<int> triple = new List<int>();
                        var mv1 = GetActiveMemoValue(y, ec_row[x1]);
                        var mv2 = GetActiveMemoValue(y, ec_row[x2]);
                        var mv3 = GetActiveMemoValue(y, ec_row[x3]);

                        triple.AddRange(mv1);
                        triple.AddRange(mv2);
                        triple.AddRange(mv3);

                        triple = triple.Distinct().ToList();
                        triple.Sort();

                        if (triple.Count != 3) // 트리플 발견
                        {
                            continue;
                        }

                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();

                        foreach (var ec in ec_row)
                        {
                            if (ec == ec_row[x1] || ec == ec_row[x2] || ec == ec_row[x3])
                            {
                                hc.Add(objects[y, ec]);
                                continue;
                            }
                            else
                            {
                                foreach (var single in triple)
                                {
                                    if (IsInMemoCell(y, ec, single))
                                    {
                                        dc.Add(memoObjects[y, ec, ValToY(single), ValToX(single)]);
                                    }
                                }
                            }
                        }
                        if (dc.Count != 0)
                        {
                            breaker = true;

                            //대사
                            string[] str = { "네이키드 트리플",
                                $"{y+1} 행에서 강조된 셀들은 {triple[0]}, {triple[1]}, {triple[2]} 세 값으로만 구성되어 있습니다..",
                                $"즉,  {triple[0]}, {triple[1]}, {triple[2]} 세 값은 이 세 셀들에서만 존재해야 합니다.",
                                $"따라서 다른 셀들에 있는 {triple[0]}, {triple[1]}, {triple[2]} 값들을 지웁니다."
                            };

                            //처방
                            var hclist = MakeHCList(null, hc, hc, dc);
                            var hb = MakeBundle(y);
                            var hbl = MakeBundleList(null, hb, hb, hb);

                            hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hbl);
                            return;
                        }
                    }
                }
            }
        }

        //col
        for (int x = 0; x < 9; x++)
        {
            var ec_col = GetEmptyCellsInCol(x); //빈 좌표 모음
            var ec_count = ec_col.Count;
            if (ec_count < 5)
            {
                continue;
            }

            for (int y1 = 0; y1 < ec_count - 2; y1++)
            {
                for (int y2 = y1 + 1; y2 < ec_count - 1; y2++)
                {
                    for (int y3 = y2 + 1; y3 < ec_count; y3++)
                    {
                        List<int> triple = new List<int>();
                        var mv1 = GetActiveMemoValue(ec_col[y1], x);
                        var mv2 = GetActiveMemoValue(ec_col[y2], x);
                        var mv3 = GetActiveMemoValue(ec_col[y3], x);

                        triple.AddRange(mv1);
                        triple.AddRange(mv2);
                        triple.AddRange(mv3);

                        triple = triple.Distinct().ToList();
                        triple.Sort();

                        if (triple.Count != 3)
                        {
                            continue;
                        }
                        //트리플 발견
                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();

                        foreach (var ec in ec_col)
                        {
                            if (ec == ec_col[y1] || ec == ec_col[y2] || ec == ec_col[y3])
                            {
                                hc.Add(objects[ec, x]);
                                continue;
                            }
                            else
                            {
                                foreach (var single in triple)
                                {
                                    if (IsInMemoCell(ec, x, single))
                                    {
                                        dc.Add(memoObjects[ec, x, ValToY(single), ValToX(single)]);
                                    }
                                }
                            }
                        }
                        if (dc.Count != 0)
                        {
                            breaker = true;

                            //대사
                            string[] str = { "네이키드 트리플",
                            $"{x+1} 열에서 강조된 셀들은 {triple[0]}, {triple[1]}, {triple[2]} 세 값으로만 구성되어 있습니다..",
                            $"즉,  {triple[0]}, {triple[1]}, {triple[2]} 세 값은 이 세 셀들에서만 존재해야 합니다.",
                            $"따라서 다른 셀들에 있는 {triple[0]}, {triple[1]}, {triple[2]} 값들을 지웁니다."};

                            //처방
                            var hclist = MakeHCList(null, hc, hc, dc);
                            var hb = MakeBundle(9 + x);
                            var hbl = MakeBundleList(null, hb, hb, hb);

                            hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hbl);
                            return;
                        }
                    }
                }
            }
        }

        //sg
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var ec_sg = GetEmptyCellsInSG(y, x); //빈 좌표 모음
                var ec_count = ec_sg.Count;

                if (ec_count < 5)
                {
                    continue;
                }

                for (int sg1 = 0; sg1 < ec_count - 2; sg1++)
                {
                    for (int sg2 = sg1 + 1; sg2 < ec_count - 1; sg2++)
                    {
                        for (int sg3 = sg2 + 1; sg3 < ec_count; sg3++)
                        {
                            List<int> triple = new List<int>();
                            var mv1 = GetActiveMemoValue(ec_sg[sg1].Item1, ec_sg[sg1].Item2);
                            var mv2 = GetActiveMemoValue(ec_sg[sg2].Item1, ec_sg[sg2].Item2);
                            var mv3 = GetActiveMemoValue(ec_sg[sg3].Item1, ec_sg[sg3].Item2);

                            triple.AddRange(mv1);
                            triple.AddRange(mv2);
                            triple.AddRange(mv3);

                            triple = triple.Distinct().ToList();
                            triple.Sort();

                            if (triple.Count != 3)
                            {
                                continue;
                            }
                            //트리플 발견
                            List<GameObject> hc = new List<GameObject>();
                            List<GameObject> dc = new List<GameObject>();

                            foreach (var ec in ec_sg)
                            {
                                if (ec == ec_sg[sg1] || ec == ec_sg[sg2] || ec == ec_sg[sg3])
                                {
                                    hc.Add(objects[ec.Item1, ec.Item2]);
                                    continue;
                                }
                                else
                                {
                                    foreach (var single in triple)
                                    {
                                        if (IsInMemoCell(ec.Item1, ec.Item2, single))
                                        {
                                            dc.Add(memoObjects[ec.Item1, ec.Item2, ValToY(single), ValToX(single)]);
                                        }
                                    }
                                }
                            }
                            if (dc.Count != 0)
                            {
                                breaker = true;

                                //대사
                                string[] str = { "네이키드 트리플",
                                    $"{y+1}-{x+1} 서브그리드에서 강조된 셀들은 {triple[0]}, {triple[1]}, {triple[2]} 세 값으로만 구성되어 있습니다..",
                                    $"즉,  {triple[0]}, {triple[1]}, {triple[2]} 세 값은 이 세 셀들에서만 존재해야 합니다.",
                                    $"따라서 다른 셀들에 있는 {triple[0]}, {triple[1]}, {triple[2]} 값들을 지웁니다."
                                };

                                //처방
                                var hclist = MakeHCList(null, hc, hc, dc);
                                var hb = MakeBundle(18 + YXToVal(y, x));
                                var hbl = MakeBundleList(null, hb, hb, hb);

                                hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hbl);
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
                    var mcr1 = GetMemoCellInRow(y1, val);
                    var mcr2 = GetMemoCellInRow(y2, val);

                    if (mcr1.Count != 2 || mcr2.Count != 2)
                    {
                        continue;
                    }

                    if (mcr1[0] != mcr2[0] || mcr1[1] != mcr2[1])
                    {
                        continue;
                    }

                    //Xwing 발견
                    var mcc1 = GetMemoCellInCol(mcr1[0], val);
                    var mcc2 = GetMemoCellInCol(mcr1[1], val);


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
                        dc.Add(memoObjects[mc, mcr1[0], ValToY(val), ValToX(val)]);
                        hdc.Add(objects[mc, mcr1[0]]);
                    }

                    foreach (var mc in mcc2)
                    {
                        if (mc == y1 || mc == y2)
                        {
                            continue;
                        }
                        dc.Add(memoObjects[mc, mcr1[1], ValToY(val), ValToX(val)]);
                        hdc.Add(objects[mc, mcr1[1]]);
                    }


                    //대사
                    string[] str = {
                    "X-윙",
                    $"{y1+1}행과 {y2+1}행에 같은 열을 걸친 셀이 두 개씩 존재합니다.",
                    $"열 조건을 충족하기 위해 이렇게 강조된 셀들 안에 반드시 두 개의 {val} 값이 들어가야만 합니다.",
                    $"따라서 다음 셀들엔 {val} 값이 들어갈 수 없습니다."
                };

                    //처방
                    var hcList = MakeHCList(null, hc, hc, hdc);
                    var hb1 = MakeBundle(y1, y2);
                    var hb2 = MakeBundle(y1, y2, 9 + mcr1[0], 9 + mcr1[1]);
                    var hbl = MakeBundleList(null, hb1, hb2, null);
                    hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hbl);

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
                    var mcc1 = GetMemoCellInCol(x1, val);
                    var mcc2 = GetMemoCellInCol(x2, val);

                    if (mcc1.Count != 2 || mcc2.Count != 2)
                    {
                        continue;
                    }
                    if (mcc1[0] != mcc2[0] || mcc1[1] != mcc2[1])
                    {
                        continue;
                    }

                    //Xwing 발견
                    var mcr1 = GetMemoCellInRow(mcc1[0], val);
                    var mcr2 = GetMemoCellInRow(mcc1[1], val);

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
                        dc.Add(memoObjects[mcc1[0], mc, ValToY(val), ValToX(val)]);
                        hdc.Add(objects[mcc1[0], mc]);
                    }

                    foreach (var mc in mcr2)
                    {
                        if (mc == x1 || mc == x2)
                        {
                            continue;
                        }
                        dc.Add(memoObjects[mcc1[1], mc, ValToY(val), ValToX(val)]);
                        hdc.Add(objects[mcc1[1], mc]);
                    }


                    //대사
                    string[] str = {
                   "X-윙",
                    $"{x1+1}열과 {x2+1}열에 같은 행을 걸친 셀이 두 개씩 존재합니다.",
                    $"행 조건을 충족하기 위해 이렇게 강조된 셀들 안에 반드시 두 개의 {val} 값이 들어가야만 합니다.",
                    $"따라서 다음 셀들엔 {val} 값이 들어갈 수 없습니다."
                };

                    //처방
                    var hcList = MakeHCList(null, hc, hc, hdc);
                    var hb1 = MakeBundle(9 + x1, 9 + x2);
                    var hb2 = MakeBundle(9 + x1, 9 + x2, mcc1[0], mcc1[1]);
                    var hbl = MakeBundleList(null, hb1, hb2, null);
                    hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hbl);

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

    private List<int> MakeBundle(params int[] codes)
    {
        List<int> list = new List<int>();
        foreach (var code in codes)
        {
            list.Add(code);
        }
        return list;
    }

    private List<List<int>> MakeBundleList(params List<int>[] bundles)
    {
        List<List<int>> list = new List<List<int>>();
        foreach (var bundle in bundles)
        {
            list.Add(bundle);
        }
        return list;
    }


    private Tuple<(int, int), int> MakeTuple((int, int) YX, int value)
    {
        Tuple<(int, int), int> tuple = new Tuple<(int, int), int>(YX, value);
        return tuple;
    }
}
