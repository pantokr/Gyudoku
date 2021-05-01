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

    private List<Tuple<GameObject, GameObject>> tmp_hint = new List<Tuple<GameObject, GameObject>>();
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
        RunBasicHint();
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

        FindFromFullSudoku();
        if (breaker)
        {
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
                tmp_hint.Clear();
                tmp_hint.Add(null);
                tmp_hint.Add(new Tuple<GameObject, GameObject>(objects[cell.y, cell.x], objects[cell.y, cell.x]));

                //ó��
                List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                toFill.Add(new Tuple<Vector2Int, int>(cell, val));

                hintDialogManager.StartDialog(str, tmp_hint, toFill);
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
                tmp_hint.Clear();
                tmp_hint.Add(null);
                tmp_hint.Add(new Tuple<GameObject, GameObject>(objects[cell.y, cell.x], objects[cell.y, cell.x]));

                //ó��
                List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                toFill.Add(new Tuple<Vector2Int, int>(cell, val));

                hintDialogManager.StartDialog(str, tmp_hint, toFill);
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
                    tmp_hint.Clear();
                    tmp_hint.Add(null);
                    tmp_hint.Add(new Tuple<GameObject, GameObject>(objects[cell.y, cell.x], objects[cell.y, cell.x]));

                    //ó��
                    List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                    toFill.Add(new Tuple<Vector2Int, int>(cell, val));

                    hintDialogManager.StartDialog(str, tmp_hint, toFill);
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
                        string[] str = { "���� �̱�", $"���� �࿡�� �� ���� �� �� �ִ� ���� {val + 1}�ϳ��Դϴ�." };
                        tmp_hint.Clear();
                        tmp_hint.Add(null);
                        tmp_hint.Add(new Tuple<GameObject, GameObject>(
                            objects[_y, _x], objects[_y, _x]));

                        //ó��
                        List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                        toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                        hintDialogManager.StartDialog(str, tmp_hint, toFill);
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
                        tmp_hint.Clear();
                        tmp_hint.Add(null);
                        tmp_hint.Add(new Tuple<GameObject, GameObject>(
                            objects[_y, _x], objects[_y, _x]));

                        //ó��
                        List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                        toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                        hintDialogManager.StartDialog(str, tmp_hint, toFill);
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
                            tmp_hint.Clear();
                            tmp_hint.Add(null);
                            tmp_hint.Add(new Tuple<GameObject, GameObject>(
                                objects[_y, _x], objects[_y, _x]));

                            //ó��
                            List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                            toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val + 1));

                            hintDialogManager.StartDialog(str, tmp_hint, toFill);
                            return true;

                        }
                    }
                }
            }
        }

        return false;
    }

    public bool FindNakedSingle(bool isAutoSingle = false)
    {
        for (int _y = 0; _y < 9; _y++)
        {
            for (int _x = 0; _x < 9; _x++)
            {

            }
        }
        return false;
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
                    string[] str = { "�������", $"{_y + 1}�� {_x + 1}������ {val}�� ���� �մϴ�." };

                    tmp_hint.Clear();
                    tmp_hint.Add(null);
                    tmp_hint.Add(new Tuple<GameObject, GameObject>(
                        objects[_y, _x], objects[_y, _x]));

                    //ó��
                    List<Tuple<Vector2Int, int>> toFill = new List<Tuple<Vector2Int, int>>();
                    toFill.Add(new Tuple<Vector2Int, int>(new Vector2Int(_x, _y), val));
                }
            }
        }
    }
}
