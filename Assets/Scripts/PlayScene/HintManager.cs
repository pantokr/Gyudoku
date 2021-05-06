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
                string[] _str = { "������ �ֽ��ϴ�." };
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

        FindNakedPair();
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
            string[] str = { "�� �̻� ��Ʈ�� �����ϴ�.\n" +
                "(�޸� �۵��ǰ� �ִٸ� �ٸ� ��Ʈ�� ���� ���� �ֽ��ϴ�.)"};
            hintDialogManager.StartDialog(str);
            return;
        }
    }

    public void RunBasicHint()
    {
        // ���� �˻�
        var p1 = sudokuController.CompareWithFullSudoku();
        if (p1.Count != 0)
        {
            breaker = true;

            //���
            string[] str = { "������ �ֽ��ϴ�.", "������ �����մϴ�." };
            hintDialogManager.StartDialog(str);

            //ó��
            foreach (var point in p1)
            {
                cellManager.DeleteCell(point.Item1, point.Item2);
            }

        }
        if (breaker)
        {
            return;
        }

        //�޸� ��� �˻�
        var p2 = sudokuController.CompareMemoWithFullSudoku();
        if (p2.Count != 0)
        {
            breaker = true;

            //���
            string[] str = { "�޸� ������մϴ�.", "�޸� �����մϴ�." };
            hintDialogManager.StartDialog(str);

            //ó��
            autoMemoManager.RunAutoMemo();
        }
        if (breaker)
        {
            return;
        }
    }

    private void FindFullHouse() //Ǯ�Ͽ콺
    {
        //row �˻�
        for (int _y = 0; _y < 9; _y++)
        {
            var ev = sudokuController.GetEmptyValueInRow(_y);
            if (ev.Count == 1)
            {
                int _x = sudokuController.GetEmptyCellsInRow(_y)[0];
                int val = ev[0];
                breaker = true;

                //���
                string[] str = { "Ǯ �Ͽ콺", $"���� �࿡ �� �� {ev[0]}�� ��� �ֽ��ϴ�." };

                //ó��
                var hc = MakeHC(null, objects[_y, _x]);
                var toFill = MakeTuple((_y, _x), val);

                hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                return;
            }
        }

        //col �˻�
        for (int _x = 0; _x < 9; _x++)
        {
            var ev = sudokuController.GetEmptyValueInCol(_x);
            if (ev.Count == 1)
            {
                int _y = sudokuController.GetEmptyCellsInCol(_x)[0];
                int val = ev[0];
                breaker = true;

                //���
                string[] str = { "Ǯ �Ͽ콺", $"���� ���� �� �� {ev[0]}�� ��� �ֽ��ϴ�." };

                //ó��
                var hc = MakeHC(null, objects[_y, _x]);
                var toFill = MakeTuple((_y, _x), val);

                hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                return;
            }
        }

        //����׸���
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

                    //���
                    string[] str = { "Ǯ �Ͽ콺", $"����׸��忡 �� �� {ev[0]}�� ��� �ֽ��ϴ�." };

                    //ó��
                    var hc = MakeHC(null, objects[_y, _x]);
                    var toFill = MakeTuple((_y, _x), val);

                    hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                    return;
                }
            }
        }
    }

    public bool FindHiddenSingle(bool isAutoSingle = false) //���� �̱�
    {
        //row �˻�
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
                    else // �Ϲ�
                    {
                        //���
                        string[] str = { "���� �̱�", $"���� �࿡�� �� ���� �� �� �ִ� ���� {val} �ϳ��Դϴ�." };

                        //ó��
                        var hc = MakeHC(null, objects[_y, _x]);
                        var toFill = MakeTuple((_y, _x), val);

                        hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                        return true;

                    }
                }
            }
        }

        //col �˻�
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
                    else // �Ϲ�
                    {
                        //���
                        string[] str = { "���� �̱�", $"���� ������ �� ���� �� �� �ִ� ���� {val} �ϳ��Դϴ�." };

                        //ó��
                        var hc = MakeHC(null, objects[_y, _x]);
                        var toFill = MakeTuple((_y, _x), val);

                        hintDialogManager.StartDialogAndFillCell(str, hc, toFill);
                        return true;

                    }
                }
            }
        }

        //SG �˻�
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
                        else // �Ϲ�
                        {
                            //���
                            string[] str = { "���� �̱�", $"3X3 ����׸��忡�� �� ���� �� �� �ִ� ���� {val} �ϳ��Դϴ�." };

                            //ó��
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

                        //������
                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();
                        for (int _x = 0; _x < 9; _x++)
                        {
                            if (_x >= x * 3 && _x < x * 3 + 3) //������ ������ �ƴ� ��
                            {
                                if (sudokuController.IsInMemoCell(r, _x, ev))
                                {
                                    hc.Add(memoObjects[r, _x, (ev - 1) / 3, (ev - 1) % 3]);
                                }
                                continue;
                            }

                            if (sudokuController.IsInMemoCell(r, _x, ev) == true) //������ ������ �� 
                            {
                                dc.Add(memoObjects[r, _x, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }

                        if (dc.Count != 0)
                        {
                            breaker = true;

                            //���
                            string[] str = {
                                "���ͼ���", $"{r + 1}���� (����׸��� ������ �����ϱ� ����)������ ���� �� �ϳ��� ������ {ev} ���� ���� �մϴ�",
                                $"���� �� ���鿡�� {ev} ���� �� �� �����ϴ�."
                            };

                            //ó��
                            var hcList = MakeHCList(null, hc, dc);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);
                            return;
                        }
                    }

                    //cols
                    if (cols.Count == 1)
                    {
                        int c = cols[0];

                        //������
                        List<GameObject> hc = new List<GameObject>();
                        List<GameObject> dc = new List<GameObject>();
                        for (int _y = 0; _y < 9; _y++)
                        {
                            if (_y >= y * 3 && _y < y * 3 + 3) //������ ������ �ƴ� ��
                            {
                                if (sudokuController.IsInMemoCell(_y, c, ev))
                                {
                                    hc.Add(memoObjects[_y, c, (ev - 1) / 3, (ev - 1) % 3]);
                                }
                                continue;
                            }

                            if (sudokuController.IsInMemoCell(_y, c, ev) == true) //������ ������ �� 
                            {
                                dc.Add(memoObjects[_y, c, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }

                        if (dc.Count != 0)
                        {
                            breaker = true;

                            //���
                            string[] str = {
                                "���ͼ���", $"{c + 1}���� (����׸��� ������ �����ϱ� ����)������ ���� �� �ϳ��� ������ {ev} ���� ���� �մϴ�",
                                $"���� �� ���鿡�� {ev} ���� �� �� �����ϴ�."
                            };

                            //ó��
                            var hcList = MakeHCList(null, hc, dc);
                            hintDialogManager.StartDialogAndDeleteMemo(str, hcList, dc);
                            return;
                        }
                    }
                }
            }
        }
    }// ���ͼ���

    public bool FindNakedSingle(bool isAutoSingle = false)
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
                        else // �Ϲ�
                        {
                            //���
                            string[] str = { "����Ű�� �̱�", $"�� ���� {val} ����� �� �� �ִ� ���� �����ϴ�." };

                            //ó��
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

    private void FindNakedPair()
    {
        //row
        for (int y = 0; y < 9; y++)
        {
            var emptyXList = sudokuController.GetEmptyCellsInRow(y); //��� �ִ� x��ǥ
            var mvList = sudokuController.GetMemoValuesInRow(y); //�޸� �ȿ� ����ִ� ������ ����
            for (int _x1 = 0; _x1 < emptyXList.Count - 1; _x1++)
            {
                for (int _x2 = _x1 + 1; _x2 < emptyXList.Count; _x2++)
                {
                    if (mvList[_x1].Count != 2 || mvList[_x2].Count != 2)
                    {
                        continue;
                    }

                    if (sudokuController.IsEqualMemoCell((y, emptyXList[_x1]), (y, emptyXList[_x2]))) // ����Ű�� ��� �߰�
                    {
                        var mvs = mvList[_x1]; // ���õ� ���� �ȿ� ����ִ� �޸𰪵�

                        // ���õ� ���� �ȿ� ����ִ� �޸𰪵�
                        List<GameObject> dc = new List<GameObject>();

                        foreach (var emptyX in emptyXList) // ����Ű�� ��� ���� ������ ���� ����
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
                                hc.Add(memoObjects[y, emptyXList[_x1], (mv - 1) / 3, (mv - 1) % 3]);
                                hc.Add(memoObjects[y, emptyXList[_x2], (mv - 1) / 3, (mv - 1) % 3]);
                            }

                            //���
                            string[] str = {
                                "����Ű�� ���",
                            $"{y+1} ���� ������ �� ���� �� {mvs[0]}, {mvs[1]}���� �̷���� �Ȱ��� ������ �޸� �����Դϴ�.",
                            $"�̴� �� {mvs[0]}, {mvs[1]}�� �� ���� ������ �� �������� �����ؾ߸� �Ѵٴ� ���� �ǹ��մϴ�.",
                            "���� ���� �޸� ������ �����մϴ�."};

                            //ó��
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
            var emptyYList = sudokuController.GetEmptyCellsInCol(x); //��� �ִ� y��ǥ
            var mvList = sudokuController.GetMemoValuesInCol(x); //�޸� �ȿ� ����ִ� ������ ����
            for (int _y1 = 0; _y1 < emptyYList.Count - 1; _y1++)
            {
                for (int _y2 = _y1 + 1; _y2 < emptyYList.Count; _y2++)
                {
                    if (mvList[_y1].Count != 2 || mvList[_y2].Count != 2)
                    {
                        continue;
                    }

                    if (sudokuController.IsEqualMemoCell((emptyYList[_y1], x), (emptyYList[_y2], x)))
                    { // ����Ű�� ��� �߰�                    {
                        var mvs = mvList[_y1]; // ���õ� ���� �ȿ� ����ִ� �޸𰪵�

                        // ���õ� ���� �ȿ� ����ִ� �޸𰪵�
                        List<GameObject> dc = new List<GameObject>();

                        foreach (var emptyY in emptyYList) // ����Ű�� ��� ���� ������ ���� ����
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

                            //���
                            string[] str = {
                                "����Ű�� ���",
                            $"{x+1} ���� ������ �� ���� �� {mvs[0]}, {mvs[1]}���� �̷���� �Ȱ��� ������ �޸� �����Դϴ�.",
                            $"�̴� �� {mvs[0]}, {mvs[1]}�� �� ���� ������ �� �������� �����ؾ߸� �Ѵٴ� ���� �ǹ��մϴ�.",
                            "���� ���� �޸� ������ �����մϴ�."};

                            //ó��
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
                var emptyYXList = sudokuController.GetEmptyCellsInSG(y, x); //��� �ִ� y��ǥ
                var mvList = sudokuController.GetMemoValuesInSG(y, x); //�޸� �ȿ� ����ִ� ������ ����
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
                            (emptyYXList[_sg2].Item1, emptyYXList[_sg2].Item2))) // ����Ű�� ��� �߰�
                        {
                            var mvs = mvList[_sg1]; // ���õ� ���� �ȿ� ����ִ� �޸𰪵�

                            // ���õ� ���� �ȿ� ����ִ� �޸𰪵�
                            List<GameObject> dc = new List<GameObject>();

                            foreach (var emptyYX in emptyYXList) // ����Ű�� ��� ���� ������ ���� ����
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

                                //���
                                string[] str = {
                                "����Ű�� ���",
                            $"{y*3+x+1} ��° ����׸����� ������ �� ���� �� {mvs[0]}, {mvs[1]}���� �̷���� �Ȱ��� ������ �޸� �����Դϴ�.",
                            $"�̴� �� {mvs[0]}, {mvs[1]}�� �� ���� ������ �� �������� �����ؾ߸� �Ѵٴ� ���� �ǹ��մϴ�.",
                            "���� ���� �޸� ������ �����մϴ�."};

                                //ó��
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

                    //���
                    string[] str = { "�������", $"{_y + 1}�� {_x + 1}���� ���� {val} �Դϴ�." };

                    //ó��
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
