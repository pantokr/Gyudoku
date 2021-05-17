using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : SudokuController
{
    public SudokuController sudokuController;
    public HintDialogManager hintDialogManager;

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


        if (!IsNormalSudoku())
        {
            string[] _str = { "오류가 있습니다." };
            hintDialogManager.StartDialog(_str);
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

        FindNakedQuad();
        if (breaker)
        {
            return;
        }

        FindSimpleColorLink3();
        if (breaker)
        {
            return;
        }

        FindXYWing();
        if (breaker)
        {
            return;
        }

        FindSwordFish();
        if (breaker)
        {
            return;
        }

        FindHiddenTriple();
        if (breaker)
        {
            return;
        }

        FindSimpleColorLink5();
        if (breaker)
        {
            return;
        }

        FindXChain();
        if (breaker)
        {
            return;
        }

        FindWWing();
        if (breaker)
        {
            return;
        }

        FindJellyFish();
        if (breaker)
        {
            return;
        }

        FindFinnedXWing();
        if (breaker)
        {
            return;
        }

        FindXYZWing();
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

        string[] str = { "더 이상 힌트가 없습니다.\n" +
                "(메모가 작동되고 있다면 다른 힌트를 얻을 수도 있습니다.)"};
        hintDialogManager.StartDialog(str);
        return;

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
                    var toFill = MakeTuple((cell.Item1, cell.Item2), val);

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
                            string[] str = { "네이키드 싱글", $"이 셀에 {val} 값 말고는 들어갈 수 있는 값이 없습니다." };

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
    }// 인터섹션

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

                    if (IsEqualMemoCell(new Tuple<int, int>(y, emptyXList[_x1]), new Tuple<int, int>(y, emptyXList[_x2]))) // 네이키드 페어 발견
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
                            $"{y+1} 행의 강조된 두 셀에 {mvs[0]}, {mvs[1]} 값으로 이루어진 똑같은 구성의 메모 셀들입니다.",
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

                    if (IsEqualMemoCell(new Tuple<int, int>(emptyYList[_y1], x), new Tuple<int, int>(emptyYList[_y2], x)))
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
                            $"{x+1} 열의 강조된 두 셀에 {mvs[0]}, {mvs[1]} 값으로 이루어진 똑같은 구성의 메모 셀들입니다.",
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
                            new Tuple<int, int>(emptyYXList[_sg1].Item1, emptyYXList[_sg1].Item2),
                            new Tuple<int, int>(emptyYXList[_sg2].Item1, emptyYXList[_sg2].Item2))) // 네이키드 페어 발견
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
                            $"{y+1}-{x+1} 서브그리드의 강조된 두 셀은 {mvs[0]}, {mvs[1]} 값으로 이루어진 똑같은 구성의 메모 셀들입니다.",
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
    } //네이키드 페어

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
                        var hbl = MakeBundleList(null, hb, hb, hb);

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
                        var hbl = MakeBundleList(null, hb, hb, hb);

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
                            var hbl = MakeBundleList(null, hb, hb, hb);

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
    }//네이키드 트리플

    private void FindNakedQuad() //네이키드 쿼드
    {
        //row
        for (int y = 0; y < 9; y++)
        {
            var ec_row = GetEmptyCellsInRow(y); //빈 좌표 모음
            var ec_count = ec_row.Count;
            if (ec_count < 6)
            {
                continue;
            }

            for (int x1 = 0; x1 < ec_count - 3; x1++)
            {
                for (int x2 = x1 + 1; x2 < ec_count - 2; x2++)
                {
                    for (int x3 = x2 + 1; x3 < ec_count - 1; x3++)
                    {
                        for (int x4 = x3 + 1; x4 < ec_count; x4++)
                        {
                            List<int> quad = new List<int>();
                            var mv1 = GetActiveMemoValue(y, ec_row[x1]);
                            var mv2 = GetActiveMemoValue(y, ec_row[x2]);
                            var mv3 = GetActiveMemoValue(y, ec_row[x3]);
                            var mv4 = GetActiveMemoValue(y, ec_row[x4]);

                            quad.AddRange(mv1);
                            quad.AddRange(mv2);
                            quad.AddRange(mv3);
                            quad.AddRange(mv4);

                            quad = quad.Distinct().ToList();
                            quad.Sort();

                            if (quad.Count != 4) // 트리플 발견
                            {
                                continue;
                            }

                            List<GameObject> hc = new List<GameObject>();
                            List<GameObject> dc = new List<GameObject>();

                            foreach (var ec in ec_row)
                            {
                                if (ec == ec_row[x1] || ec == ec_row[x2] || ec == ec_row[x3] || ec == ec_row[x4])
                                {
                                    hc.Add(objects[y, ec]);
                                    continue;
                                }
                                else
                                {
                                    foreach (var single in quad)
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
                                string[] str = { "네이키드 쿼드",
                                $"{y+1} 행에서 강조된 셀들은 {quad[0]}, {quad[1]}, {quad[2]}, {quad[3]} 네 값으로만 구성되어 있습니다..",
                                $"즉, {quad[0]}, {quad[1]}, {quad[2]}, {quad[3]} 네 값은 이 네 셀들에서만 존재해야 합니다.",
                                $"따라서 다른 셀들에 있는 {quad[0]}, {quad[1]}, {quad[2]}, {quad[3]} 값들을 지웁니다."
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
        }

        //col
        for (int x = 0; x < 9; x++)
        {
            var ec_col = GetEmptyCellsInCol(x); //빈 좌표 모음
            var ec_count = ec_col.Count;
            if (ec_count < 6)
            {
                continue;
            }

            for (int y1 = 0; y1 < ec_count - 3; y1++)
            {
                for (int y2 = y1 + 1; y2 < ec_count - 2; y2++)
                {
                    for (int y3 = y2 + 1; y3 < ec_count - 1; y3++)
                    {
                        for (int y4 = y3 + 1; y4 < ec_count; y4++)
                        {
                            List<int> quad = new List<int>();
                            var mv1 = GetActiveMemoValue(ec_col[y1], x);
                            var mv2 = GetActiveMemoValue(ec_col[y2], x);
                            var mv3 = GetActiveMemoValue(ec_col[y3], x);
                            var mv4 = GetActiveMemoValue(ec_col[y4], x);

                            quad.AddRange(mv1);
                            quad.AddRange(mv2);
                            quad.AddRange(mv3);
                            quad.AddRange(mv4);

                            quad = quad.Distinct().ToList();
                            quad.Sort();

                            if (quad.Count != 4)
                            {
                                continue;
                            }
                            //트리플 발견
                            List<GameObject> hc = new List<GameObject>();
                            List<GameObject> dc = new List<GameObject>();

                            foreach (var ec in ec_col)
                            {
                                if (ec == ec_col[y1] || ec == ec_col[y2] || ec == ec_col[y3] || ec == ec_col[y4])
                                {
                                    hc.Add(objects[ec, x]);
                                    continue;
                                }
                                else
                                {
                                    foreach (var single in quad)
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
                                string[] str = { "네이키드 쿼드",
                                    $"{x+1} 열에서 강조된 셀들은 {quad[0]}, {quad[1]}, {quad[2]}, {quad[3]} 네 값으로만 구성되어 있습니다..",
                                    $"즉, {quad[0]}, {quad[1]}, {quad[2]}, {quad[3]} 네 값은 이 네 셀들에서만 존재해야 합니다.",
                                    $"따라서 다른 셀들에 있는 {quad[0]}, {quad[1]}, {quad[2]}, {quad[3]} 값들을 지웁니다."};

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
        }

        //sg
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var ec_sg = GetEmptyCellsInSG(y, x); //빈 좌표 모음
                var ec_count = ec_sg.Count;

                if (ec_count < 6)
                {
                    continue;
                }

                for (int sg1 = 0; sg1 < ec_count - 3; sg1++)
                {
                    for (int sg2 = sg1 + 1; sg2 < ec_count - 2; sg2++)
                    {
                        for (int sg3 = sg2 + 1; sg3 < ec_count - 1; sg3++)
                        {
                            for (int sg4 = sg3 + 1; sg4 < ec_count; sg4++)
                            {
                                List<int> quad = new List<int>();
                                var mv1 = GetActiveMemoValue(ec_sg[sg1].Item1, ec_sg[sg1].Item2);
                                var mv2 = GetActiveMemoValue(ec_sg[sg2].Item1, ec_sg[sg2].Item2);
                                var mv3 = GetActiveMemoValue(ec_sg[sg3].Item1, ec_sg[sg3].Item2);
                                var mv4 = GetActiveMemoValue(ec_sg[sg4].Item1, ec_sg[sg4].Item2);

                                quad.AddRange(mv1);
                                quad.AddRange(mv2);
                                quad.AddRange(mv3);
                                quad.AddRange(mv4);

                                quad = quad.Distinct().ToList();
                                quad.Sort();

                                if (quad.Count != 4)
                                {
                                    continue;
                                }
                                //쿼드 발견
                                List<GameObject> hc = new List<GameObject>();
                                List<GameObject> dc = new List<GameObject>();

                                foreach (var ec in ec_sg)
                                {
                                    if (ec == ec_sg[sg1] || ec == ec_sg[sg2] || ec == ec_sg[sg3] || ec == ec_sg[sg4])
                                    {
                                        hc.Add(objects[ec.Item1, ec.Item2]);
                                        continue;
                                    }
                                    else
                                    {
                                        foreach (var single in quad)
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
                                    string[] str = { "네이키드 쿼드",
                                    $"{y+1}-{x+1} 서브그리드에서 강조된 셀들은 {quad[0]}, {quad[1]}, {quad[2]}, {quad[3]} 네 값으로만 구성되어 있습니다..",
                                    $"즉, {quad[0]}, {quad[1]}, {quad[2]}, {quad[3]} 네 값은 이 네 셀들에서만 존재해야 합니다.",
                                    $"따라서 다른 셀들에 있는 {quad[0]}, {quad[1]}, {quad[2]}, {quad[3]} 값들을 지웁니다."
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
    } //X wing

    private void FindXYWing()
    {
        for (int y = 0; y < 9; y++)
        {
            var ecr = GetEmptyCellsInRow(y);
            var ecr_count = ecr.Count;

            for (int x = 0; x < ecr_count; x++)
            {
                var amv = GetActiveMemoValue(y, ecr[x]);
                if (amv.Count != 2)
                {
                    continue;
                }

                var l_v1 = GetLinkedCell(y, ecr[x], amv[0], -1);
                var l_v2 = GetLinkedCell(y, ecr[x], amv[1], -1);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (l_v1[i] == null || l_v2[j] == null || i == j)
                        {
                            continue;
                        }

                        var l_c1 = l_v1[i];
                        var l_c2 = l_v2[j];

                        var amc_c1 = GetActiveMemoValue(l_c1.Item1, l_c1.Item2);
                        var amc_c2 = GetActiveMemoValue(l_c2.Item1, l_c2.Item2);

                        if (amc_c1.Count != 2 || amc_c2.Count != 2)
                        {
                            continue;
                        }
                        var dv = GetDuplicatedValueByTwoCell(new Tuple<int, int>(l_c1.Item1, l_c1.Item2), new Tuple<int, int>(l_c2.Item1, l_c2.Item2));

                        if (dv.Count != 1 || dv[0] == amv[0] || dv[0] == amv[1])
                        {
                            continue;
                        }

                        var dup = GetDuplicatedCellByTwoCell(new Tuple<int, int>(l_c1.Item1, l_c1.Item2), new Tuple<int, int>(l_c2.Item1, l_c2.Item2));
                        List<GameObject> hdc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();

                        List<(GameObject, GameObject)> hl2 = new List<(GameObject, GameObject)>();
                        foreach (var d in dup)
                        {
                            if (IsInMemoCell(d.Item1, d.Item2, dv[0]))
                            {
                                hdc.Add(objects[d.Item1, d.Item2]);
                                dc.Add(memoObjects[d.Item1, d.Item2, ValToY(dv[0]), ValToX(dv[0])]);

                                hl2.Add((memoObjects[l_c1.Item1, l_c1.Item2, ValToY(dv[0]), ValToX(dv[0])], memoObjects[d.Item1, d.Item2, ValToY(dv[0]), ValToX(dv[0])]));
                                hl2.Add((memoObjects[l_c2.Item1, l_c2.Item2, ValToY(dv[0]), ValToX(dv[0])], memoObjects[d.Item1, d.Item2, ValToY(dv[0]), ValToX(dv[0])]));
                            }
                        }

                        if (hdc.Count == 0)
                        {
                            continue;
                        }

                        breaker = true;

                        string[] str = { "XY-윙",
                            $"{y+1}행 {ecr[x]+1}열을 기준으로 {l_c1.Item1+1}행 {l_c1.Item2+1}열은 값 {amv[0]}에 대한 링크, {l_c2.Item1+1}행 {l_c2.Item2+1}열은 값 {amv[1]}에 대한 링크 셀입니다.",
                            $"두 링크 셀 중 하나에는 {dv[0]} 값이 들어가야만 합니다.",
                            $"따라서 다음 셀에는 {dv[0]} 값이 들어갈 수 없습니다."};

                        var hc1 = MakeHC(
                            memoObjects[y, ecr[x], ValToY(amv[0]), ValToX(amv[0])],
                            memoObjects[l_c1.Item1, l_c1.Item2, ValToY(amv[0]), ValToX(amv[0])],
                            memoObjects[y, ecr[x], ValToY(amv[1]), ValToX(amv[1])],
                            memoObjects[l_c2.Item1, l_c2.Item2, ValToY(amv[1]), ValToX(amv[1])],
                            objects[y, ecr[x]]
                            );
                        var hl1 = MakeHL((memoObjects[y, ecr[x], ValToY(amv[0]), ValToX(amv[0])], memoObjects[l_c1.Item1, l_c1.Item2, ValToY(amv[0]), ValToX(amv[0])]),
                             (memoObjects[y, ecr[x], ValToY(amv[1]), ValToX(amv[1])], memoObjects[l_c2.Item1, l_c2.Item2, ValToY(amv[1]), ValToX(amv[1])]));

                        var hc2 = MakeHC(objects[l_c1.Item1, l_c1.Item2], objects[l_c2.Item1, l_c2.Item2]);

                        var hcList = MakeHCList(null, hc1, hc2, hdc);
                        var hlList = MakeHLList(null, hl1, null, hl2);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hlList, null);

                    }
                }
            }
        }
    } //XY wing

    private void FindSimpleColorLink3()
    {
        for (int y = 0; y < 9; y++)
        {
            var mcr = GetEmptyCellsInRow(y);//[0,4]
            var mcr_count = mcr.Count;
            for (int _x = 0; _x < mcr_count; _x++) // 빈 셀 X 좌표
            {
                var amv = GetActiveMemoValue(y, mcr[_x]);

                foreach (var mv in amv)
                {
                    var tuple = GetLinkedCellRecursive(y, mcr[_x], mv, 0, 3, -1); //마지막 링크 셀

                    if (tuple != null)
                    {
                        var dup = GetDuplicatedCellByTwoCell(new Tuple<int, int>(y, mcr[_x]), new Tuple<int, int>(tuple.Item1, tuple.Item2));

                        List<GameObject> dc = new List<GameObject>();
                        List<GameObject> hdc = new List<GameObject>();

                        foreach (var d in dup) //그 셀
                        {
                            if (IsInMemoCell(d.Item1, d.Item2, mv))
                            {
                                dc.Add(memoObjects[d.Item1, d.Item2, ValToY(mv), ValToX(mv)]);
                                hdc.Add(objects[d.Item1, d.Item2]);
                            }
                        }

                        if (dc.Count == 0)
                        {
                            continue;
                        }

                        breaker = true;

                        List<GameObject> hc = new List<GameObject>();
                        List<(GameObject, GameObject)> hl = new List<(GameObject, GameObject)>();

                        var tracer = base.tracer;
                        tracer.Reverse();

                        for (int i = 0; i < tracer.Count; i++)
                        {
                            var t = tracer[i];
                            hc.Add(objects[t.Item1, t.Item2]);

                            if (i < tracer.Count - 1) //hintline group
                            {
                                var tp = tracer[i + 1];
                                hl.Add((memoObjects[t.Item1, t.Item2, ValToY(mv), ValToX(mv)], memoObjects[tp.Item1, tp.Item2, ValToY(mv), ValToX(mv)]));
                            }
                        }

                        //hintline 1
                        List<(GameObject, GameObject)> thl1 = new List<(GameObject, GameObject)>();
                        thl1.Add(hl[0]);

                        //hintline 2
                        List<(GameObject, GameObject)> thl2 = new List<(GameObject, GameObject)>();
                        foreach (var d in dup)
                        {
                            if (IsInMemoCell(d.Item1, d.Item2, mv))
                            {
                                thl2.Add((memoObjects[y, mcr[_x], ValToY(mv), ValToX(mv)], memoObjects[d.Item1, d.Item2, ValToY(mv), ValToX(mv)])); //시작셀
                                thl2.Add((memoObjects[tracer[tracer.Count - 1].Item1, tracer[tracer.Count - 1].Item2, ValToY(mv), ValToX(mv)], memoObjects[d.Item1, d.Item2, ValToY(mv), ValToX(mv)])); //끝셀
                            }
                        }

                        List<GameObject> thc = MakeHC(hc[0], hc[1]);

                        //대사
                        string[] str = { "심플 컬러 링크",
                            $"{mv} 값으로 이루어진 두 셀은 한 셀에 {mv} 값이 들어가면 다른 쪽에는 들어갈 수 없는 링크 관계입니다.",
                            $"연결된 링크를 이렇게 세 쌍을 발견했습니다.",
                            $"링크에 어떻게 {mv} 값을 구성하든지 링크의 첫 셀과 끝 셀의 공유 셀에는 {mv} 값이 들어갈 수 없습니다."};

                        //처방
                        var hcList = MakeHCList(null, thc, hc, hdc);
                        var hlList = MakeHLList(null, thl1, hl, thl2);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hlList);

                        return;
                    }
                }
            }
        }
    } //심플 컬러 (링크 3)

    private void FindSwordFish()
    {
        //row
        for (int y1 = 0; y1 < 7; y1++)
        {
            for (int y2 = y1 + 1; y2 < 8; y2++)
            {
                for (int y3 = y2 + 1; y3 < 9; y3++)
                {
                    for (int val = 1; val <= 9; val++)
                    {
                        var mcr1 = GetMemoCellInRow(y1, val);
                        var mcr2 = GetMemoCellInRow(y2, val);
                        var mcr3 = GetMemoCellInRow(y3, val);

                        if ((mcr1.Count < 2 || mcr1.Count > 3) ||
                            (mcr2.Count < 2 || mcr2.Count > 3) ||
                            (mcr3.Count < 2 || mcr3.Count > 3))
                        {
                            continue;
                        }

                        List<int> cols = new List<int>();
                        foreach (var mcr in mcr1)
                        {
                            cols.Add(mcr);
                        }
                        foreach (var mcr in mcr2)
                        {
                            cols.Add(mcr);
                        }
                        foreach (var mcr in mcr3)
                        {
                            cols.Add(mcr);
                        }

                        cols = cols.Distinct().ToList();
                        cols.Sort();

                        if (cols.Count != 3) // sword fish 발견
                        {
                            continue;
                        }


                        List<GameObject> dc = new List<GameObject>();
                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> hdc = new List<GameObject>();
                        foreach (var col in cols)
                        {
                            var mcc = GetMemoCellInCol(col, val);
                            foreach (var r in mcc) //모든 빈 가로 행 탐색
                            {
                                if (IsInMemoCell(r, col, val))
                                {
                                    if (r == y1 || r == y2 || r == y3)
                                    {
                                        hc.Add(objects[r, col]);
                                    }
                                    else
                                    {
                                        dc.Add(memoObjects[r, col, ValToY(val), ValToX(val)]);
                                        hdc.Add(objects[r, col]);
                                    }
                                }
                            }
                        }

                        if (dc.Count == 0)
                        {
                            continue;
                        }
                        //
                        breaker = true;

                        //대사
                        string[] str = { "스워드피쉬",
                            $"{y1+1}, {y2+1}, {y3+1} 세 행에서 각 행에 3개 이하의 {val} 값이 들어갈 수 있습니다.",
                            $"이 셀들은 또한 세 개의 열 안에 속하기도 합니다.",
                            $"이는 이 셀들 안에서 각 행과 열에 하나씩 총 세 개의 {val} 값이 구성됨을 의미합니다.",
                            $"따라서 {cols[0]+1}, {cols[1]+1}, {cols[2]+1}의 다른 셀에 있는 {val} 값을 지울 수 있습니다."};

                        //처방

                        var hcList = MakeHCList(null, hc, hc, hc, hdc);
                        var b1 = MakeBundle(y1, y2, y3);
                        var b2 = MakeBundle(9 + cols[0], 9 + cols[1], 9 + cols[2]);
                        var bl = MakeBundleList(null, b1, b2, b2, b2);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, bl);

                        return;
                    }
                }
            }
        }

        //col
        for (int x1 = 0; x1 < 7; x1++)
        {
            for (int x2 = x1 + 1; x2 < 8; x2++)
            {
                for (int x3 = x2 + 1; x3 < 9; x3++)
                {
                    for (int val = 1; val <= 9; val++)
                    {
                        var mcc1 = GetMemoCellInCol(x1, val);
                        var mcc2 = GetMemoCellInCol(x2, val);
                        var mcc3 = GetMemoCellInCol(x3, val);

                        if ((mcc1.Count < 2 || mcc1.Count > 3) ||
                            (mcc2.Count < 2 || mcc2.Count > 3) ||
                            (mcc3.Count < 2 || mcc3.Count > 3))
                        {
                            continue;
                        }

                        List<int> rows = new List<int>();
                        foreach (var mcc in mcc1)
                        {
                            rows.Add(mcc);
                        }
                        foreach (var mcc in mcc2)
                        {
                            rows.Add(mcc);
                        }
                        foreach (var mcc in mcc3)
                        {
                            rows.Add(mcc);
                        }

                        rows = rows.Distinct().ToList();
                        rows.Sort();

                        if (rows.Count != 3) // sword fish 발견
                        {
                            continue;
                        }


                        List<GameObject> dc = new List<GameObject>();
                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> hdc = new List<GameObject>();
                        foreach (var row in rows)
                        {
                            var mcr = GetMemoCellInRow(row, val);
                            foreach (var c in mcr) //모든 빈 가로 행 탐색
                            {
                                if (IsInMemoCell(row, c, val))
                                {
                                    if (c == x1 || c == x2 || c == x3)
                                    {
                                        hc.Add(objects[row, c]);
                                    }
                                    else
                                    {
                                        dc.Add(memoObjects[row, c, ValToY(val), ValToX(val)]);
                                        hdc.Add(objects[row, c]);
                                    }
                                }
                            }
                        }

                        if (dc.Count == 0)
                        {
                            continue;
                        }
                        //
                        breaker = true;

                        //대사
                        string[] str = { "스워드피쉬",
                            $"{x1+1}, {x2+1}, {x3+1} 세 열에서 각 열에 3개 이하의 {val} 값이 들어갈 수 있습니다.",
                            $"이 셀들은 또한 세 개의 행 안에 속하기도 합니다.",
                            $"이는 이 셀들 안에서 각 행과 열에 하나씩 총 세 개의 {val} 값이 구성됨을 의미합니다.",
                            $"따라서 {rows[0]+1}, {rows[1]+1}, {rows[2]+1}의 다른 셀에 있는 {val} 값을 지울 수 있습니다."};

                        //처방

                        var hcList = MakeHCList(null, hc, hc, hc, hdc);
                        var b1 = MakeBundle(9 + x1, 9 + x2, 9 + x3);
                        var b2 = MakeBundle(rows[0], rows[1], rows[2]);
                        var bl = MakeBundleList(null, b1, b2, b2, b2);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, bl);

                        return;
                    }
                }
            }
        }
    }//스워드 피쉬

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

                    return;
                }
            }
        }
    }

    private void FindHiddenTriple() //히든 트리플
    {
        //row 검사
        for (int _y = 0; _y < 9; _y++)
        {
            var evr = GetEmptyValueInRow(_y);
            var evr_cnt = evr.Count; // 
            for (int _x1 = 0; _x1 < evr_cnt - 2; _x1++)
            {
                for (int _x2 = _x1 + 1; _x2 < evr_cnt - 1; _x2++)
                {
                    for (int _x3 = _x2 + 1; _x3 < evr_cnt; _x3++)
                    {
                        var ev1_row = GetMemoCellInRow(_y, evr[_x1]); //값이 들어있는 x 좌표 리스트
                        var ev2_row = GetMemoCellInRow(_y, evr[_x2]);
                        var ev3_row = GetMemoCellInRow(_y, evr[_x3]);

                        if ((ev1_row.Count > 3 || ev1_row.Count < 2) || (ev2_row.Count > 3 || ev2_row.Count < 2) || (ev3_row.Count > 3 || ev3_row.Count < 2))
                        {
                            continue;
                        }

                        List<int> xlist = new List<int>();
                        foreach (var ev in ev1_row)
                        {
                            xlist.Add(ev);
                        }
                        foreach (var ev in ev2_row)
                        {
                            xlist.Add(ev);
                        }
                        foreach (var ev in ev3_row)
                        {
                            xlist.Add(ev);
                        }

                        xlist = xlist.Distinct().ToList();
                        xlist.Sort();

                        if (xlist.Count != 3)
                        {
                            continue;
                        }

                        int[] triple = { evr[_x1], evr[_x2], evr[_x3] }; //트리플 발견

                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();

                        foreach (var ev in xlist) // x 좌표
                        {
                            var mvs = GetActiveMemoValue(_y, ev);
                            foreach (var mv in mvs) // 셀의 모든 메모값 
                            {
                                if (mv == triple[0] || mv == triple[1] || mv == triple[2])
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
                            string[] str = { "히든 트리플",
                            $"{_y + 1}행에서 {triple[0]}, {triple[1]}, {triple[2]} 값이 이 세 셀에만 존재합니다.",
                            $"즉, 이 세 셀에는 {triple[0]}, {triple[1]}, {triple[2]} 값만 들어갈 수 있습니다.",
                            "따라서 다른 값들은 이 셀에 들어갈 수 없습니다."};

                            //처방
                            var hclist = MakeHCList(null, hc, hc, dc);
                            var hb = MakeBundle(_y);
                            var hbl = MakeBundleList(null, hb, hb, hb);

                            hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hbl);

                            return;
                        }
                    }

                    //페어 값: (evr[_x1], evr[_x2]) 좌표 : (ev1_row[0], ev1_row[1])


                }
            }
        }

        //col 검사
        for (int _x = 0; _x < 9; _x++)
        {
            var evc = GetEmptyValueInCol(_x);
            var evc_cnt = evc.Count; // 
            for (int _y1 = 0; _y1 < evc_cnt - 2; _y1++)
            {
                for (int _y2 = _y1 + 1; _y2 < evc_cnt - 1; _y2++)
                {
                    for (int _y3 = _y2 + 1; _y3 < evc_cnt; _y3++)
                    {
                        var ev1_col = GetMemoCellInCol(_x, evc[_y1]); //값이 들어있는 y 좌표 리스트
                        var ev2_col = GetMemoCellInCol(_x, evc[_y2]);
                        var ev3_col = GetMemoCellInCol(_x, evc[_y3]);

                        if ((ev1_col.Count > 3 || ev1_col.Count < 2) || (ev2_col.Count > 3 || ev2_col.Count < 2) || (ev3_col.Count > 3 || ev3_col.Count < 2))
                        {
                            continue;
                        }

                        List<int> ylist = new List<int>();
                        foreach (var ev in ev1_col)
                        {
                            ylist.Add(ev);
                        }
                        foreach (var ev in ev2_col)
                        {
                            ylist.Add(ev);
                        }
                        foreach (var ev in ev3_col)
                        {
                            ylist.Add(ev);
                        }

                        ylist = ylist.Distinct().ToList();
                        ylist.Sort();

                        if (ylist.Count != 3)
                        {
                            continue;
                        }

                        int[] triple = { evc[_y1], evc[_y2], evc[_y3] }; //트리플 발견

                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();

                        foreach (var ev in ylist) // x 좌표
                        {
                            var mvs = GetActiveMemoValue(ev, _x);
                            foreach (var mv in mvs) // 셀의 모든 메모값 
                            {
                                if (mv == triple[0] || mv == triple[1] || mv == triple[2])
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
                            string[] str = { "히든 트리플",
                            $"{_x + 1}열에서 {triple[0]}, {triple[1]}, {triple[2]} 값이 이 세 셀에만 존재합니다.",
                            $"즉, 이 세 셀에는 {triple[0]}, {triple[1]}, {triple[2]} 값만 들어갈 수 있습니다.",
                            "따라서 다른 값들은 이 셀에 들어갈 수 없습니다."};

                            //처방
                            var hclist = MakeHCList(null, hc, hc, dc);
                            var hb = MakeBundle(9 + _x);
                            var hbl = MakeBundleList(null, hb, hb, hb);

                            hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hbl);

                            return;
                        }
                    }

                    //페어 값: (evr[_x1], evr[_x2]) 좌표 : (ev1_row[0], ev1_row[1])


                }
            }
        }

        //sg 검사
        for (int _y = 0; _y < 3; _y++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                var evsg = GetEmptyValueInSG(_y, _x);
                var evsg_cnt = evsg.Count; // 

                for (int _sg1 = 0; _sg1 < evsg_cnt - 2; _sg1++)
                {
                    for (int _sg2 = _sg1 + 1; _sg2 < evsg_cnt - 1; _sg2++)
                    {
                        for (int _sg3 = _sg2 + 1; _sg3 < evsg_cnt; _sg3++)
                        {
                            var ev1_sg = GetMemoCellInSG(_y, _x, evsg[_sg1]); //값이 들어있는 y 좌표 리스트
                            var ev2_sg = GetMemoCellInSG(_y, _x, evsg[_sg2]);
                            var ev3_sg = GetMemoCellInSG(_y, _x, evsg[_sg3]);

                            if ((ev1_sg.Count > 3 || ev1_sg.Count < 2) || (ev2_sg.Count > 3 || ev2_sg.Count < 2) || (ev3_sg.Count > 3 || ev3_sg.Count < 2))
                            {
                                continue;
                            }

                            List<Tuple<int, int>> sglist = new List<Tuple<int, int>>();
                            foreach (var ev in ev1_sg)
                            {
                                sglist.Add(ev);
                            }
                            foreach (var ev in ev2_sg)
                            {
                                sglist.Add(ev);
                            }
                            foreach (var ev in ev3_sg)
                            {
                                sglist.Add(ev);
                            }

                            sglist = sglist.Distinct().ToList();
                            sglist.Sort();

                            if (sglist.Count != 3)
                            {
                                continue;
                            }

                            int[] triple = { evsg[_sg1], evsg[_sg2], evsg[_sg3] }; //트리플 발견

                            List<GameObject> hc = new List<GameObject>();
                            List<GameObject> dc = new List<GameObject>();

                            foreach (var ev in sglist) // x 좌표
                            {
                                var mvs = GetActiveMemoValue(ev.Item1, ev.Item2);
                                foreach (var mv in mvs) // 셀의 모든 메모값 
                                {
                                    if (mv == triple[0] || mv == triple[1] || mv == triple[2])
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
                                string[] str = { "히든 트리플",
                            $"{_y + 1}-{_x+1}서브그리드에서 {triple[0]}, {triple[1]}, {triple[2]} 값이 이 세 셀에만 존재합니다.",
                            $"즉, 이 세 셀에는 {triple[0]}, {triple[1]}, {triple[2]} 값만 들어갈 수 있습니다.",
                            "따라서 다른 값들은 이 셀에 들어갈 수 없습니다."};

                                //처방
                                var hclist = MakeHCList(null, hc, hc, dc);
                                var hb = MakeBundle(18 + _y * 3 + _x);
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

    private void FindSimpleColorLink5()
    {
        for (int y = 0; y < 9; y++)
        {
            var mcr = GetEmptyCellsInRow(y);//[0,4]
            var mcr_count = mcr.Count;
            for (int _x = 0; _x < mcr_count; _x++) // 빈 셀 X 좌표
            {
                var amv = GetActiveMemoValue(y, mcr[_x]);

                foreach (var mv in amv)
                {
                    var tuple = GetLinkedCellRecursive(y, mcr[_x], mv, 0, 5, -1); //마지막 링크 셀
                    if (tuple != null)
                    {
                        var dup = GetDuplicatedCellByTwoCell(new Tuple<int, int>(y, mcr[_x]), new Tuple<int, int>(tuple.Item1, tuple.Item2));

                        List<GameObject> dc = new List<GameObject>();
                        List<GameObject> hdc = new List<GameObject>();

                        foreach (var d in dup) //그 셀
                        {
                            if (IsInMemoCell(d.Item1, d.Item2, mv))
                            {
                                dc.Add(memoObjects[d.Item1, d.Item2, ValToY(mv), ValToX(mv)]);
                                hdc.Add(objects[d.Item1, d.Item2]);
                            }
                        }

                        if (dc.Count == 0)
                        {
                            continue;
                        }

                        breaker = true;

                        List<GameObject> hc = new List<GameObject>();
                        List<(GameObject, GameObject)> hl = new List<(GameObject, GameObject)>();

                        var tracer = base.tracer;
                        tracer.Reverse();
                        //print("--");
                        for (int i = 0; i < tracer.Count; i++)
                        {
                            //print(tracer[i].Item1.ToString() + tracer[i].Item2.ToString());
                            var t = tracer[i];
                            hc.Add(objects[t.Item1, t.Item2]);

                            if (i < tracer.Count - 1) //hintline group
                            {
                                var tp = tracer[i + 1];
                                hl.Add((memoObjects[t.Item1, t.Item2, ValToY(mv), ValToX(mv)], memoObjects[tp.Item1, tp.Item2, ValToY(mv), ValToX(mv)]));
                            }
                        }

                        //hintline 1
                        List<(GameObject, GameObject)> thl1 = new List<(GameObject, GameObject)>();
                        thl1.Add(hl[0]);

                        //hintline 2
                        List<(GameObject, GameObject)> thl2 = new List<(GameObject, GameObject)>();
                        foreach (var d in dup)
                        {
                            if (IsInMemoCell(d.Item1, d.Item2, mv))
                            {
                                thl2.Add((memoObjects[y, mcr[_x], ValToY(mv), ValToX(mv)], memoObjects[d.Item1, d.Item2, ValToY(mv), ValToX(mv)])); //시작셀
                                thl2.Add((memoObjects[tracer[tracer.Count - 1].Item1, tracer[tracer.Count - 1].Item2, ValToY(mv), ValToX(mv)], memoObjects[d.Item1, d.Item2, ValToY(mv), ValToX(mv)])); //끝셀
                            }
                        }

                        List<GameObject> thc = MakeHC(hc[0], hc[1]);

                        //대사
                        string[] str = { "심플 컬러 링크",
                            $"{mv} 값으로 이루어진 두 셀은 한 셀에 {mv} 값이 들어가면 다른 쪽에는 들어갈 수 없는 링크 관계입니다.",
                            $"연결된 링크를 이렇게 다섯 쌍을 발견했습니다.",
                            $"링크에 어떻게 {mv} 값을 구성하든지 링크의 첫 셀과 끝 셀의 공유 셀에는 {mv} 값이 들어갈 수 없습니다."};

                        //처방
                        var hcList = MakeHCList(null, thc, hc, hdc);
                        var hlList = MakeHLList(null, thl1, hl, thl2);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, hlList);

                        return;
                    }
                }
            }
        }
    } //심플 컬러 (링크 5)

    private void FindXChain()
    {
        for (int y = 0; y < 9; y++)
        {
            var ecr = GetEmptyCellsInRow(y);
            foreach (var ec in ecr)
            {
                var amv = GetActiveMemoValue(y, ec);
                foreach (var mv in amv)
                {
                    GetXOChainRecursiveWithTracer(y, ec, mv, 0, 1);

                    var tracerList = base.tracerList;

                    if (tracerList.Count == 0)
                    {
                        continue;
                    }

                    foreach (var tracer in tracerList)
                    {
                        if (IsTwoCellInSameArea(tracer[0], tracer[3]))
                        {
                            continue;
                        }
                        else
                        {
                            var dupc = GetDuplicatedCellByTwoCell(new Tuple<int, int>(y, ec), new Tuple<int, int>(tracer[3].Item1, tracer[3].Item2));
                            List<GameObject> dc = new List<GameObject>();
                            List<(GameObject, GameObject)> end_hl = new List<(GameObject, GameObject)>();
                            foreach (var dup in dupc)
                            {
                                if (IsInMemoCell(dup.Item1, dup.Item2, mv))
                                {
                                    dc.Add(memoObjects[dup.Item1, dup.Item2, ValToY(mv), ValToX(mv)]);

                                    end_hl.Add((memoObjects[tracer[0].Item1, tracer[0].Item2, ValToY(mv), ValToX(mv)],
                                        memoObjects[dup.Item1, dup.Item2, ValToY(mv), ValToX(mv)]));
                                    end_hl.Add((memoObjects[tracer[3].Item1, tracer[3].Item2, ValToY(mv), ValToX(mv)],
                                        memoObjects[dup.Item1, dup.Item2, ValToY(mv), ValToX(mv)]));
                                }
                            }

                            if (dc.Count == 0)
                            {
                                continue;
                            }

                            List<GameObject> hcs = new List<GameObject>();
                            List<(GameObject, GameObject)> hls = new List<(GameObject, GameObject)>();
                            for (int i = 0; i < 4; i++)
                            {
                                hcs.Add(objects[tracer[i].Item1, tracer[i].Item2]);
                                if (i == 3)
                                {
                                    break;
                                }
                                hls.Add((memoObjects[tracer[i].Item1, tracer[i].Item2, ValToY(mv), ValToX(mv)],
                                    memoObjects[tracer[i + 1].Item1, tracer[i + 1].Item2, ValToY(mv), ValToX(mv)]));
                            }

                            List<GameObject> hc1 = new List<GameObject>()
                            {
                                memoObjects[tracer[1].Item1, tracer[1].Item2, ValToY(mv), ValToX(mv)]
                            };
                            List<GameObject> hc2 = new List<GameObject>()
                            {
                                memoObjects[tracer[2].Item1, tracer[2].Item2, ValToY(mv), ValToX(mv)]
                            };

                            List<(GameObject, GameObject)> hl1 = new List<(GameObject, GameObject)>()
                            {
                                (memoObjects[tracer[0].Item1, tracer[0].Item2, ValToY(mv), ValToX(mv)],
                                memoObjects[tracer[1].Item1, tracer[1].Item2, ValToY(mv), ValToX(mv)])
                            };
                            List<(GameObject, GameObject)> hl2 = new List<(GameObject, GameObject)>()
                            {
                                (memoObjects[tracer[1].Item1, tracer[1].Item2, ValToY(mv), ValToX(mv)],
                                memoObjects[tracer[2].Item1, tracer[2].Item2, ValToY(mv), ValToX(mv)])
                            };

                            breaker = true;

                            //대사
                            string[] str = { "X-체인",
                                $"{tracer[0].Item1+1}행 {tracer[0].Item2+1}열 셀에 {mv} 값이 \"들어가지 않으면\" {tracer[1].Item1+1}행 {tracer[1].Item2+1}열 셀에는 반드시 {mv} 값이 들어갑니다.",
                                $"{tracer[1].Item1+1}행 {tracer[1].Item2+1}열 셀에 {mv} 값이 \"들어가면\" {tracer[2].Item1+1}행 {tracer[2].Item2+1}열 셀에는 {mv} 값이 들어갈 수 없습니다.",
                                $"이와 같은 체인을 계속 연장해 다음과 같은 X-O-X-O 구조의 체인을 발견했습니다.",
                                $"{tracer[0].Item1+1}행 {tracer[0].Item2+1}열 셀에 {mv} 값을 넣든 넣지 않든 강조된 셀에는 {mv} 값이 들어갈 수 없습니다." };

                            //처방

                            var hclist = MakeHCList(null, hc1, hc2, hcs, dc);
                            var hllist = MakeHLList(null, hl1, hl2, hls, end_hl);

                            hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hllist);

                            return;

                        }
                    }
                }
            }
        }
    }

    private void FindWWing()
    {
        for (int y = 0; y < 9; y++)
        {
            var ec_row = GetEmptyCellsInRow(y);
            foreach (var ec in ec_row) // x좌표 반환
            {
                var amv = GetActiveMemoValue(y, ec);
                if (amv.Count != 2)
                {
                    continue;
                }
                foreach (var mv in amv)
                {
                    GetOXChainRecursiveWithTracer(y, ec, mv, 0, 1);
                    var tracerList = base.tracerList;

                    foreach (var tracer in tracerList)
                    {
                        if (!IsEqualMemoCell(new Tuple<int, int>(y, ec), tracer[3]))
                        {
                            continue;
                        }
                        //wwing 발견
                        int mv_alter = amv[0] == mv ? amv[1] : amv[0];

                        var dupc = GetDuplicatedCellByTwoCell(new Tuple<int, int>(y, ec), new Tuple<int, int>(tracer[3].Item1, tracer[3].Item2));
                        List<GameObject> dc = new List<GameObject>();
                        List<(GameObject, GameObject)> end_hl = new List<(GameObject, GameObject)>();
                        foreach (var dup in dupc)
                        {
                            if (IsInMemoCell(dup.Item1, dup.Item2, mv_alter))
                            {
                                dc.Add(memoObjects[dup.Item1, dup.Item2, ValToY(mv_alter), ValToX(mv_alter)]);

                                end_hl.Add((memoObjects[tracer[0].Item1, tracer[0].Item2, ValToY(mv_alter), ValToX(mv_alter)],
                                    memoObjects[dup.Item1, dup.Item2, ValToY(mv_alter), ValToX(mv_alter)]));
                                end_hl.Add((memoObjects[tracer[3].Item1, tracer[3].Item2, ValToY(mv_alter), ValToX(mv_alter)],
                                    memoObjects[dup.Item1, dup.Item2, ValToY(mv_alter), ValToX(mv_alter)]));
                            }
                        }

                        if (dc.Count == 0)
                        {
                            continue;
                        }

                        breaker = true;

                        List<GameObject> hcs = new List<GameObject>();
                        List<(GameObject, GameObject)> hls = new List<(GameObject, GameObject)>();
                        for (int i = 0; i < 4; i++)
                        {
                            hcs.Add(memoObjects[tracer[i].Item1, tracer[i].Item2, ValToY(mv), ValToX(mv)]);
                            if (i == 3)
                            {
                                break;
                            }
                            hls.Add((memoObjects[tracer[i].Item1, tracer[i].Item2, ValToY(mv), ValToX(mv)],
                                memoObjects[tracer[i + 1].Item1, tracer[i + 1].Item2, ValToY(mv), ValToX(mv)]));
                        }

                        var hc1 = MakeHC(objects[y, ec], objects[tracer[3].Item1, tracer[3].Item2]);
                        var hc2 = MakeHC(memoObjects[y, ec, ValToY(mv_alter), ValToX(mv_alter)], memoObjects[tracer[3].Item1, tracer[3].Item2, ValToY(mv_alter), ValToX(mv_alter)]);


                        //대사
                        string[] str = { "W-윙",
                            $"{y+1}행 {ec+1}열의 셀과 {tracer[3].Item1+1}행 {tracer[3].Item2+1}열의 셀은 둘 다 {mv} 값과 {mv_alter} 값으로 구성되어 있습니다.",
                            $"만약 {y+1}행 {ec+1}열에 {mv} 값이 들어간다면 다음과 같은 O-X-O-X 체인이 생겨 {tracer[3].Item1+1}행 {tracer[3].Item2+1}열에는 {mv} 값이 들어갈 수 없습니다.",
                            $"즉 {y+1}행 {ec+1}열의 셀과 {tracer[3].Item1+1}행 {tracer[3].Item2+1}열의 셀 중 하나에는 {mv_alter} 값이 들어가야만 합니다.",
                            $"따라서 강조된 셀에는 {mv_alter} 값이 들어갈 수 없습니다."
                        };

                        //처방
                        var hclist = MakeHCList(null, hc1, hcs, hc2, dc);
                        var hllist = MakeHLList(null, null, hls, null, end_hl);

                        hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hllist);
                        return;
                    }
                }
            }
        }
    }

    private void FindFinnedXWing()
    {

    }

    private void FindXYZWing()
    {
        for (int y = 0; y < 9; y++)
        {
            var ec_row = GetEmptyCellsInRow(y);
            foreach (var ec in ec_row)
            {
                var amv = GetActiveMemoValue(y, ec); //amv는 세 값이 들어있음
                if (amv.Count != 3)
                {
                    continue;
                }
                List<Tuple<int, int>> sub_mv = new List<Tuple<int, int>>() // 부분집합, 집합 요소 개수는 모두 두 개 
                {
                    new Tuple<int, int>(amv[0], amv[1]),
                    new Tuple<int, int>(amv[1], amv[2]),
                    new Tuple<int, int>(amv[0], amv[2])
                };

                var cfa = GetCellForcingArea(y, ec);
                //SudokuManager.PrintListYX(cfa);
                List<Tuple<int, int>> xyz_wing = new List<Tuple<int, int>>();
                foreach (var c in cfa) // 가능한 좌표들
                {
                    var c_amv = GetActiveMemoValue(c.Item1, c.Item2); // 그 좌표들의 값들
                    if (c_amv.Count != 2)
                    {
                        continue;
                    }

                    foreach (var sm in sub_mv) // 
                    {
                        if (new Tuple<int, int>(c_amv[0], c_amv[1]).Equals(sm))
                        {
                            xyz_wing.Add(c);
                            break;
                        }
                    }

                }

                for (int i1 = 0; i1 < xyz_wing.Count - 1; i1++)
                {
                    for (int i2 = i1 + 1; i2 < xyz_wing.Count; i2++)
                    {
                        var c_i1 = xyz_wing[i1];
                        var c_i2 = xyz_wing[i2];
                        if (IsEqualMemoCell(c_i1, c_i2))
                        {
                            continue;
                        }
                        //xyz 발견
                        var dup_val = GetDuplicatedValueByTwoCell(xyz_wing[i1], xyz_wing[i2])[0];

                        var dup_c1 = GetDuplicatedCellByTwoCell(new Tuple<int, int>(y, ec), c_i1);
                        var dup_c2 = GetDuplicatedCellByTwoCell(new Tuple<int, int>(y, ec), c_i2);

                        var dup_c_list = GetDuplicatedCellsByTwoList(dup_c1, dup_c2);
                        List<GameObject> dc = new List<GameObject>();
                        List<(GameObject, GameObject)> hl = new List<(GameObject, GameObject)>();
                        foreach (var dup_c in dup_c_list)
                        {
                            if (IsInMemoCell(dup_c.Item1, dup_c.Item2, dup_val))
                            {
                                dc.Add(memoObjects[dup_c.Item1, dup_c.Item2, ValToY(dup_val), ValToX(dup_val)]);
                                hl.Add((memoObjects[y, ec, ValToY(dup_val), ValToX(dup_val)], memoObjects[dup_c.Item1, dup_c.Item2, ValToY(dup_val), ValToX(dup_val)]));
                                hl.Add((memoObjects[c_i1.Item1, c_i1.Item2, ValToY(dup_val), ValToX(dup_val)], memoObjects[dup_c.Item1, dup_c.Item2, ValToY(dup_val), ValToX(dup_val)]));
                                hl.Add((memoObjects[c_i2.Item1, c_i2.Item2, ValToY(dup_val), ValToX(dup_val)], memoObjects[dup_c.Item1, dup_c.Item2, ValToY(dup_val), ValToX(dup_val)]));
                            }
                        }

                        if (dc.Count == 0)
                        {
                            continue;
                        }
                        breaker = true;
                        int c_i1_val = 0;
                        int c_i2_val = 0;
                        foreach (var mv in amv)
                        {
                            if (mv == dup_val)
                            {
                                continue;
                            }

                            if (IsInMemoCell(c_i1.Item1, c_i1.Item2, mv))
                            {
                                c_i1_val = mv;
                                continue;
                            }
                            if (IsInMemoCell(c_i2.Item1, c_i2.Item2, mv))
                            {
                                c_i2_val = mv;
                                continue;
                            }
                        }

                        var hc = MakeHC(objects[y, ec], objects[c_i1.Item1, c_i1.Item2], objects[c_i2.Item1, c_i2.Item2]);
                        var hc1 = MakeHC(memoObjects[y, ec, ValToY(dup_val), ValToX(dup_val)], memoObjects[c_i1.Item1, c_i1.Item2, ValToY(dup_val), ValToX(dup_val)]);
                        var hc2 = MakeHC(memoObjects[y, ec, ValToY(dup_val), ValToX(dup_val)], memoObjects[c_i2.Item1, c_i2.Item2, ValToY(dup_val), ValToX(dup_val)]);
                        var hc3 = new List<GameObject>();
                        hc3.AddRange(hc);
                        hc3.AddRange(dc);
                        string[] str = { "XYZ-윙",
                                    $"강조된 셀들 중 하나는 세 값의 메모가 들어있고, 다른 강조된 셀에는 그 중 두 값의 서로 다른 집합의 메모가 들어있습니다.",
                                    $"만약 {y+1}행 {ec+1}열에 {c_i1_val} 값이 들어가면 {c_i1.Item1+1}행 {c_i1.Item2+1}열에는 {dup_val} 값이 들어갑니다.",
                                    $"만약 {y+1}행 {ec+1}열에 {c_i1_val} 값이 들어가면 {c_i2.Item1+1}행 {c_i2.Item2+1}열에는 {dup_val} 값이 들어갑니다.",
                                    $"즉 이 세 셀들 중 하나에는 반드시 {dup_val} 값이 들어가야만 합니다.",
                                    $"따라서 세 셀들과 공유되는 영역을 가진 셀에는 {dup_val} 값이 들어갈 수 없습니다."
                                };

                        var hclist = MakeHCList(null, hc, hc1, hc2, hc, hc3);
                        var hllist = MakeHLList(null, null, null, null, null, hl);
                        hintDialogManager.StartDialogAndDeleteMemo(str, hclist, dc, hllist);
                        return;
                    }
                }
            }
        }
    }

    private void FindJellyFish()
    {
        //row
        for (int y1 = 0; y1 < 6; y1++)
        {
            for (int y2 = y1 + 1; y2 < 7; y2++)
            {
                for (int y3 = y2 + 1; y3 < 8; y3++)
                {
                    for (int y4 = y3 + 1; y4 < 9; y4++)
                    {
                        for (int val = 1; val <= 9; val++)
                        {
                            var mcr1 = GetMemoCellInRow(y1, val);
                            var mcr2 = GetMemoCellInRow(y2, val);
                            var mcr3 = GetMemoCellInRow(y3, val);
                            var mcr4 = GetMemoCellInRow(y4, val);

                            if ((mcr1.Count < 2 || mcr1.Count > 4) ||
                                (mcr2.Count < 2 || mcr2.Count > 4) ||
                                (mcr3.Count < 2 || mcr3.Count > 4) ||
                                (mcr4.Count < 2 || mcr4.Count > 4))
                            {
                                continue;
                            }

                            List<int> cols = new List<int>();
                            foreach (var mcr in mcr1)
                            {
                                cols.Add(mcr);
                            }
                            foreach (var mcr in mcr2)
                            {
                                cols.Add(mcr);
                            }
                            foreach (var mcr in mcr3)
                            {
                                cols.Add(mcr);
                            }
                            foreach (var mcr in mcr4)
                            {
                                cols.Add(mcr);
                            }

                            cols = cols.Distinct().ToList();
                            cols.Sort();

                            if (cols.Count != 4) // jelly fish 발견
                            {
                                continue;
                            }

                            List<GameObject> dc = new List<GameObject>();
                            List<GameObject> hc = new List<GameObject>();
                            List<GameObject> hdc = new List<GameObject>();
                            foreach (var col in cols)
                            {
                                var mcc = GetMemoCellInCol(col, val);
                                foreach (var r in mcc) //모든 빈 가로 행 탐색
                                {
                                    if (IsInMemoCell(r, col, val))
                                    {
                                        if (r == y1 || r == y2 || r == y3 || r == y4)
                                        {
                                            hc.Add(objects[r, col]);
                                        }
                                        else
                                        {
                                            dc.Add(memoObjects[r, col, ValToY(val), ValToX(val)]);
                                            hdc.Add(objects[r, col]);
                                        }
                                    }
                                }
                            }

                            if (dc.Count == 0)
                            {
                                continue;
                            }
                            //
                            breaker = true;

                            //대사
                            string[] str = { "젤리피쉬",
                            $"{y1+1}, {y2+1}, {y3+1}, {y4+1} 네 행에서 각 행에 4개 이하의 {val} 값이 들어갈 수 있습니다.",
                            $"이 셀들은 또한 네 개의 열 안에 속하기도 합니다.",
                            $"이는 이 셀들 안에서 각 행과 열에 하나씩 총 네 개의 {val} 값이 구성됨을 의미합니다.",
                            $"따라서 {cols[0]+1}, {cols[1]+1}, {cols[2]+1}, {cols[3]+1}의 다른 셀에 있는 {val} 값을 지울 수 있습니다."};

                            //처방

                            var hcList = MakeHCList(null, hc, hc, hc, hdc);
                            var b1 = MakeBundle(y1, y2, y3);
                            var b2 = MakeBundle(9 + cols[0], 9 + cols[1], 9 + cols[2], 9 + cols[3]);
                            var bl = MakeBundleList(null, b1, b2, b2, b2);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, bl);

                            return;
                        }
                    }
                }
            }
        }

        //col
        for (int x1 = 0; x1 < 6; x1++)
        {
            for (int x2 = x1 + 1; x2 < 7; x2++)
            {
                for (int x3 = x2 + 1; x3 < 8; x3++)
                {
                    for (int x4 = x3 + 1; x4 < 9; x4++)
                    {
                        for (int val = 1; val <= 9; val++)
                        {
                            var mcc1 = GetMemoCellInCol(x1, val);
                            var mcc2 = GetMemoCellInCol(x2, val);
                            var mcc3 = GetMemoCellInCol(x3, val);
                            var mcc4 = GetMemoCellInCol(x4, val);

                            if ((mcc1.Count < 2 || mcc1.Count > 4) ||
                                (mcc2.Count < 2 || mcc2.Count > 4) ||
                                (mcc3.Count < 2 || mcc3.Count > 4) ||
                                (mcc4.Count < 2 || mcc4.Count > 4))
                            {
                                continue;
                            }

                            List<int> rows = new List<int>();
                            foreach (var mcc in mcc1)
                            {
                                rows.Add(mcc);
                            }
                            foreach (var mcc in mcc2)
                            {
                                rows.Add(mcc);
                            }
                            foreach (var mcc in mcc3)
                            {
                                rows.Add(mcc);
                            }

                            rows = rows.Distinct().ToList();
                            rows.Sort();

                            if (rows.Count != 3) // sword fish 발견
                            {
                                continue;
                            }

                            List<GameObject> dc = new List<GameObject>();
                            List<GameObject> hc = new List<GameObject>();
                            List<GameObject> hdc = new List<GameObject>();
                            foreach (var row in rows)
                            {
                                var mcr = GetMemoCellInRow(row, val);
                                foreach (var c in mcr) //모든 빈 가로 행 탐색
                                {
                                    if (IsInMemoCell(row, c, val))
                                    {
                                        if (c == x1 || c == x2 || c == x3 || c == x4)
                                        {
                                            hc.Add(objects[row, c]);
                                        }
                                        else
                                        {
                                            dc.Add(memoObjects[row, c, ValToY(val), ValToX(val)]);
                                            hdc.Add(objects[row, c]);
                                        }
                                    }
                                }
                            }
                            if (dc.Count == 0)
                            {
                                continue;
                            }
                            //
                            breaker = true;

                            //대사
                            string[] str = { "젤리피쉬",
                            $"{x1+1}, {x2+1}, {x3+1}, {x4+1} 네 열에서 각 열에 4개 이하의 {val} 값이 들어갈 수 있습니다.",
                            $"이 셀들은 또한 네 개의 행 안에 속하기도 합니다.",
                            $"이는 이 셀들 안에서 각 행과 열에 하나씩 총 네 개의 {val} 값이 구성됨을 의미합니다.",
                            $"따라서 {rows[0]+1}, {rows[1]+1}, {rows[2]+1}, {rows[3]+1}의 다른 셀에 있는 {val} 값을 지울 수 있습니다."};

                            //처방

                            var hcList = MakeHCList(null, hc, hc, hc, hdc);
                            var b1 = MakeBundle(9 + x1, 9 + x2, 9 + x3);
                            var b2 = MakeBundle(rows[0], rows[1], rows[2]);
                            var bl = MakeBundleList(null, b1, b2, b2, b2);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc, bl);

                            return;
                        }
                    }
                }
            }
        }
    }//젤리 피쉬

    private List<GameObject> MakeHC(params GameObject[] objs)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (var obj in objs)
        {
            list.Add(obj);
        }
        return list;
    }

    private List<(GameObject, GameObject)> MakeHL(params (GameObject, GameObject)[] objs)
    {
        List<(GameObject, GameObject)> list = new List<(GameObject, GameObject)>();
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

    private List<List<(GameObject, GameObject)>> MakeHLList(params List<(GameObject, GameObject)>[] objs)
    {
        List<List<(GameObject, GameObject)>> list = new List<List<(GameObject, GameObject)>>();
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