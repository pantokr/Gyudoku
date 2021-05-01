using System;
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
    private void Start()
    {
        objects = cellManager.GetObjects();
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

        print("HINTMANAGER ERROR");
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
                int x_cell = sudokuController.GetEmptyCellInRow(_y)[0];
                int x_val = ev[0];
                breaker = true;

                //���
                string[] str = { "Ǯ �Ͽ콺", "���� �࿡ �� ���� ��� �ֽ��ϴ�." };
                tmp_hint.Clear();
                tmp_hint.Add(null);
                tmp_hint.Add(new Tuple<GameObject, GameObject>(objects[_y, x_cell], objects[_y, x_cell]));
                hintDialogManager.StartDialog(str, tmp_hint);

                //ó��
                cellManager.FillCell(_y, x_cell, x_val);
                return;
            }
        }
        
        //col �˻�
        for (int _x = 0; _x < 9; _x++)
        {
            var ev = sudokuController.GetEmptyValueInCol(_x);
            if (ev.Count == 1)
            {
                int y_cell = sudokuController.GetEmptyCellInCol(_x)[0];
                int y_val = ev[0];
                breaker = true;

                //���
                string[] str = { "Ǯ �Ͽ콺", "���� �࿡ �� ���� ��� �ֽ��ϴ�." };
                tmp_hint.Clear();
                tmp_hint.Add(null);
                tmp_hint.Add(new Tuple<GameObject, GameObject>(objects[y_cell, _x], objects[y_cell, _x]));
                hintDialogManager.StartDialog(str, tmp_hint);

                //ó��
                cellManager.FillCell(y_cell, _x, y_val);
                return;
            }
        }
    }
}
