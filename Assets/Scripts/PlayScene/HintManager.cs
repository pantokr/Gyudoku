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
    private bool breaker = false;
    private GameObject[,] objects;
    private GameObject[,,,] memoObjects;

    private int[,] sudoku;
    private void Start()
    {
        sudoku = SudokuManager.sudoku;
        objects = cellManager.GetObjects();
        memoObjects = memoManager.GetWholeMemoObjects();
    }

    public void RunHint()
    {
        breaker = false;
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

        FindCrossPointing();
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
                "(�޸� �۵��ǰ� �ִٸ� �ٸ� ��Ʈ�� ���� �� �ֽ��ϴ�.)"};
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
                Vector2Int cell = sudokuController.GetEmptyCellInRow(_y)[0];
                int val = ev[0];
                breaker = true;

                //���
                string[] str = { "Ǯ �Ͽ콺", $"���� �࿡ �� �� {ev[0]}�� ��� �ֽ��ϴ�." };
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

        //col �˻�
        for (int _x = 0; _x < 9; _x++)
        {
            var ev = sudokuController.GetEmptyValueInCol(_x);
            if (ev.Count == 1)
            {
                Vector2Int cell = sudokuController.GetEmptyCellInCol(_x)[0];
                int val = ev[0];
                breaker = true;

                //���
                string[] str = { "Ǯ �Ͽ콺", $"���� ���� �� �� {ev[0]}�� ��� �ֽ��ϴ�." };
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

        //����׸���
        for (int _y = 0; _y < 3; _y++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                var ev = sudokuController.GetEmptyValueInSG(_y, _x);
                if (ev.Count == 1)
                {
                    Vector2Int cell = sudokuController.GetEmptyCellInSG(_y, _x)[0];
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

    public bool FindHiddenSingle(bool isAutoSingle = false)
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
    } //���� �̱�

    public bool FindNakedSingle(bool isAutoSingle = false)
    {
        for (int _y = 0; _y < 9; _y++)
        {
            for (int _x = 0; _x < 9; _x++)
            {
                if (sudokuController.IsEmptyCell(_y, _x))
                {
                    List<int> vals = new List<int>();
                    for (int val = 0; val < 9; val++)
                    {
                        if (sudokuController.IsNewValueAvailableRow(_y, _x, val + 1) == true &&
                            sudokuController.IsNewValueAvailableCol(_y, _x, val + 1) == true &&
                            sudokuController.IsNewValueAvailableSG(_y, _x, val + 1) == true)
                        {
                            vals.Add(val);
                        }
                        if (vals.Count == 2)
                        {
                            break;
                        }
                    }
                    if (vals.Count == 1)
                    {
                        breaker = true;
                        int val = vals[0];

                        if (isAutoSingle)
                        {
                            cellManager.FillCell(_y, _x, val + 1);
                            return true;
                        }
                        else // �Ϲ�
                        {
                            //���
                            string[] str = { "����Ű�� �̱�", $"�� ���� {val + 1} ����� �� �� �ִ� ���� �����ϴ�." };
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
    } //�ٸ� �������� ������

    public void FindCrossPointing()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int val = 0; val < 9; val++)
                {
                    var (rows, cols) = sudokuController.GetLinesDisabledBySG(y, x, val);
                    foreach (var r_ in rows)
                    {

                        print($"y:{y + 1},x:{x + 1},val:{val + 1},r:{r_}");
                    }
                    if (rows.Count == 1)
                    {
                        int r = rows[0];
                        //print($"y:{y + 1},x:{x + 1},val:{val + 1},r:{r}");
                        List<int> crosses = new List<int>();
                        for (int _x = 0; _x < 9; _x++)
                        {
                            if (_x >= x * 3 && _x < x * 3 + 3)
                            {
                                continue;
                            }

                            if (sudoku[r, _x] == val)
                            {
                                crosses.Add(_x);
                            }
                        }

                        if (crosses.Count != 0)
                        {
                            breaker = true;
                            //���

                            string[] str = { "������(Pointing)", $"{r + 1}����  {y * 3 + x + 1}�� ° ����׸��� �ܿ� {val + 1} ���� �� �� �����ϴ�. " };
                            hintCell.Clear();
                            hintCell.Add(null);

                            //ó��
                            List<Tuple<Vector2Int, int>> toDelete = new List<Tuple<Vector2Int, int>>();
                            foreach (var cx in crosses)
                            {
                                toDelete.Add(new Tuple<Vector2Int, int>(new Vector2Int(cx, r), val + 1));
                            }

                            hintDialogManager.StartDialogAndDeleteMemo(str, hintCell, toDelete);


                            return;
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
