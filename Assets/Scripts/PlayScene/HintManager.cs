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

    private List<GameObject> hintCell = new List<GameObject>();
    private List<List<GameObject>> hintCellList = new List<List<GameObject>>();
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

        FindHiddenSingle();
        if (breaker)
        {
            return;
        }

        FindNakedSingle();
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
        bool flag;
        List<Vector2Int> points;

        // ���� �˻�
        (flag, points) = sudokuController.CompareWithFullSudoku();
        if (flag)
        {
            breaker = true;

            //���
            string[] str = { "������ �ֽ��ϴ�.", "������ �����մϴ�." };
            hintDialogManager.StartDialog(str);

            //ó��
            foreach (var point in points)
            {
                cellManager.DeleteCell(point.y, point.x);
            }

        }
        if (breaker)
        {
            return;
        }

        //�޸� ��� �˻�
        (flag, points) = sudokuController.CompareMemoWithFullSudoku();
        if (flag)
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

    private void FindFullHouse() //�� ���� �����ϸ�
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
                hintCell.Clear();
                hintCell.Add(null);
                hintCell.Add(objects[_y, _x]);

                //ó��
                List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val));

                hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
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
                hintCell.Clear();
                hintCell.Add(null);
                hintCell.Add(objects[_y, _x]);

                //ó��
                List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val));

                hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
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
                    Vector2Int cell = sudokuController.GetEmptyCellsInSG(_y, _x)[0];
                    int val = ev[0];
                    breaker = true;

                    //���
                    string[] str = { "Ǯ �Ͽ콺", $"3X3 ����׸��忡 �� �� {ev[0]}�� ��� �ֽ��ϴ�." };
                    hintCell.Clear();
                    hintCell.Add(null);
                    hintCell.Add(objects[cell.y, cell.x]);

                    //ó��
                    List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                    toFill.Add(new Tuple<Vector2Int, int>(cell, val));

                    hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                    return;
                }
            }
        }
    }

    public bool FindHiddenSingle(bool isAutoSingle = false) //���� ���� �޸� ���� �� ������ ���Ǵ� �޸�� �ϳ�
    {
        //row �˻�
        for (int val = 0; val < 9; val++)
        {
            for (int _y = 0; _y < 9; _y++)
            {
                var ev = sudokuController.GetMemoRow(val, _y);
                if (ev.Sum() == 1)
                {
                    breaker = true;
                    int _x = Array.IndexOf(ev, 1);

                    if (isAutoSingle)
                    {
                        cellManager.FillCell(_y, _x, val + 1);
                        return true;
                    }
                    else // �Ϲ�
                    {
                        //���
                        string[] str = { "���� �̱�", $"���� �࿡�� �� ���� �� �� �ִ� ���� {val + 1} �ϳ��Դϴ�." };
                        hintCell.Clear();
                        hintCell.Add(null);
                        hintCell.Add(objects[_y, _x]);

                        //ó��
                        List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                        toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                        hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                        return true;

                    }
                }
            }
        }

        //col �˻�
        for (int val = 0; val < 9; val++)
        {
            for (int _x = 0; _x < 9; _x++)
            {
                var ev = sudokuController.GetMemoCol(val, _x);
                if (ev.Sum() == 1)
                {
                    breaker = true;
                    int _y = Array.IndexOf(ev, 1);

                    if (isAutoSingle)
                    {
                        cellManager.FillCell(_y, _x, val + 1);
                        return true;
                    }
                    else // �Ϲ�
                    {
                        //���
                        string[] str = { "���� �̱�", $"���� ������ �� ���� �� �� �ִ� ���� {val + 1} �ϳ��Դϴ�." };
                        hintCell.Clear();
                        hintCell.Add(null);
                        hintCell.Add(objects[_y, _x]);

                        //ó��
                        List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                        toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                        hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                        return true;

                    }
                }
            }
        }

        //SG �˻�
        for (int val = 0; val < 9; val++)
        {
            for (int _y = 0; _y < 3; _y++)
            {
                for (int _x = 0; _x < 3; _x++)
                {
                    var ev = sudokuController.GetMemoSG(val, _y, _x);
                    if (ev.Sum() == 1)
                    {
                        breaker = true;

                        if (isAutoSingle)
                        {
                            cellManager.FillCell(_y, _x, val + 1);
                            return true;
                        }
                        else // �Ϲ�
                        {
                            //���
                            string[] str = { "���� �̱�", $"3X3 ����׸��忡�� �� ���� �� �� �ִ� ���� {val + 1} �ϳ��Դϴ�." };
                            hintCell.Clear();
                            hintCell.Add(null);
                            hintCell.Add(objects[_y, _x]);

                            //ó��
                            List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                            toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                            hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                            return true;

                        }
                    }
                }
            }
        }

        return false;
    } // ���� �̱�

    public void FindCrossPointing()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var evs = sudokuController.GetEmptyValueInSG(y, x);
                foreach (var ev in evs)
                {
                    var (rows, cols) = sudokuController.GetLinesDisabledBySG(y, x, ev - 1);
                    //rows
                    if (rows.Count == 1)
                    {
                        int r = rows[0];

                        //������
                        List<int> crosses = new List<int>();
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
                                crosses.Add(_x);
                                dc.Add(memoObjects[r, _x, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }

                        if (crosses.Count != 0)
                        {
                            breaker = true;

                            //���
                            string[] str = {
                                "������(������)", $"{r + 1}���� (����׸��� ������ �����ϱ� ����)������ ���� �� �ϳ��� ������ {ev} ���� ���� �մϴ�",
                                $"���� �� ���鿡�� {ev} ���� �� �� �����ϴ�."
                            };

                            hintCellList.Clear();
                            hintCellList.Add(null);
                            hintCellList.Add(hc);
                            hintCellList.Add(dc);

                            //ó��
                            List<GameObject> toDelete = new List<GameObject>();
                            hintDialogManager.StartDialogAndDeleteMemo(str, hintCellList, dc);
                            return;
                        }
                    }

                    //cols
                    if (cols.Count == 1)
                    {
                        int c = cols[0];

                        //������
                        List<int> crosses = new List<int>();
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
                                crosses.Add(_y);
                                dc.Add(memoObjects[_y, c, (ev - 1) / 3, (ev - 1) % 3]);
                            }
                        }

                        if (crosses.Count != 0)
                        {
                            breaker = true;

                            //���
                            string[] str = {
                                "������(������)", $"{c + 1}���� (����׸��� ������ �����ϱ� ����)������ ���� �� �ϳ��� ������ {ev} ���� ���� �մϴ�",
                                $"���� �� ���鿡�� {ev} ���� �� �� �����ϴ�."
                            };

                            hintCellList.Clear();
                            hintCellList.Add(null);
                            hintCellList.Add(hc);
                            hintCellList.Add(dc);

                            //ó��
                            List<GameObject> toDelete = new List<GameObject>();
                            hintDialogManager.StartDialogAndDeleteMemo(str, hintCellList, dc);
                            return;
                        }
                    }
                }
            }
        }
    }// ������

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
                            hintCell.Clear();
                            hintCell.Add(null);
                            hintCell.Add(objects[_y, _x]);

                            //ó��
                            List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                            toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val));

                            hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                            return true;

                        }
                    }
                }
            }
        }
        return false;
    }

    public void FindNakedPair()
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

                    if (sudokuController.IsEqualMemoCell(new Vector2Int(emptyXList[_x1], y), new Vector2Int(emptyXList[_x2], y))) // ����Ű�� ��� �߰�
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

                            hintCellList.Clear();
                            hintCellList.Add(null);
                            hintCellList.Add(hc);
                            hintCellList.Add(hc);
                            hintCellList.Add(dc);

                            //ó��
                            hintDialogManager.StartDialogAndDeleteMemo(str, hintCellList, dc);

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

                    if (sudokuController.IsEqualMemoCell(new Vector2Int(x, emptyYList[_y1]), new Vector2Int(x, emptyYList[_y2]))) // ����Ű�� ��� �߰�
                    {
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

                            hintCellList.Clear();
                            hintCellList.Add(null);
                            hintCellList.Add(hc);
                            hintCellList.Add(hc);
                            hintCellList.Add(dc);

                            //ó��
                            hintDialogManager.StartDialogAndDeleteMemo(str, hintCellList, dc);

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
                            new Vector2Int(emptyYXList[_sg1].x, emptyYXList[_sg1].y),
                            new Vector2Int(emptyYXList[_sg2].x, emptyYXList[_sg2].y))) // ����Ű�� ��� �߰�
                        {
                            var mvs = mvList[_sg1]; // ���õ� ���� �ȿ� ����ִ� �޸𰪵�

                            // ���õ� ���� �ȿ� ����ִ� �޸𰪵�
                            List<GameObject> dc = new List<GameObject>();

                            foreach (var emptyYX in emptyYXList) // ����Ű�� ��� ���� ������ ���� ����
                            {
                                if ((emptyYX.y == emptyYXList[_sg1].y) && (emptyYX.x == emptyYXList[_sg1].x) ||
                                    (emptyYX.y == emptyYXList[_sg2].y) && (emptyYX.x == emptyYXList[_sg2].x))
                                {
                                    continue;
                                }

                                foreach (var mv in mvs)
                                {
                                    if (sudokuController.IsInMemoCell(emptyYX.y, emptyYX.x, mv))
                                    {
                                        dc.Add(memoObjects[emptyYX.y, emptyYX.x, (mv - 1) / 3, (mv - 1) % 3]);
                                    }
                                }
                            }

                            if (dc.Count != 0)
                            {
                                breaker = true;

                                List<GameObject> hc = new List<GameObject>();

                                foreach (var mv in mvs)
                                {
                                    hc.Add(memoObjects[emptyYXList[_sg1].y, emptyYXList[_sg1].x, (mv - 1) / 3, (mv - 1) % 3]);
                                    hc.Add(memoObjects[emptyYXList[_sg2].y, emptyYXList[_sg2].x, (mv - 1) / 3, (mv - 1) % 3]);
                                }

                                //���
                                string[] str = {
                                "����Ű�� ���",
                            $"{y*3+x+1} ��° ����׸����� ������ �� ���� �� {mvs[0]}, {mvs[1]}���� �̷���� �Ȱ��� ������ �޸� �����Դϴ�.",
                            $"�̴� �� {mvs[0]}, {mvs[1]}�� �� ���� ������ �� �������� �����ؾ߸� �Ѵٴ� ���� �ǹ��մϴ�.",
                            "���� ���� �޸� ������ �����մϴ�."};

                                hintCellList.Clear();
                                hintCellList.Add(null);
                                hintCellList.Add(hc);
                                hintCellList.Add(hc);
                                hintCellList.Add(dc);

                                //ó��
                                hintDialogManager.StartDialogAndDeleteMemo(str, hintCellList, dc);

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

                    hintCell.Clear();
                    hintCell.Add(null);
                    hintCell.Add(objects[_y, _x]);

                    //ó��
                    List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                    toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val));
                    hintDialogManager.StartDialogAndFillCell(str, hintCell, toFill);
                }
            }
        }
    }
}
