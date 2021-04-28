using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoManager : MonoBehaviour
{
    //FB92C4 128
    public GameObject memoCell;
    public CellManager cellManager;

    public bool[,,] memoSudoku;
    private GameObject[,] objects;

    private void Start()
    {
        objects = cellManager.GetObjects();

        this.memoSudoku = SudokuManager.memoSudoku;
        AppendMemoCellObjects();
    }
    public GameObject GetMemoObject(int y, int x, int value)
    {
        GameObject parentObj = objects[y, x].transform.Find("Memo").gameObject;

        if (value == 0)
        {
            return null;
        }

        int my = (value - 1) / 3 + 1;
        int mx = (value - 1) % 3 + 1;

        return parentObj.transform.Find($"y{my}x{mx}").gameObject;
    }
    public GameObject[,] GetMemoObjects(int y, int x)
    {
        GameObject[,] retObj = new GameObject[9, 9];
        GameObject parentObj = objects[y, x].transform.Find("Memo").gameObject;
        for (int _y = 0; _y < 3; _y++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                retObj[_y, _x] = parentObj.transform.Find($"y{_y + 1}x{_x + 1}").gameObject;
            }
        }
        return retObj;
    }
    public void SetMemoSudoku(bool[,,] ms)
    {
        this.memoSudoku = (bool[,,])ms.Clone();
    }
    public void ApplyMemoSudoku(bool[,,] memoSudoku)
    {
        for (int val = 0; val < 9; val++)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (memoSudoku[val, y, x] == false)
                    {
                        DeleteMemoCell(y, x, val + 1);
                    }
                    else
                    {
                        FillMemoCell(y, x, val + 1);
                    }
                }
            }
        }
    }
    public void FillMemoCell(int y, int x, int value)
    {
        GameObject parentObj = objects[y, x].transform.Find("Memo").gameObject;

        int vy = (value - 1) / 3 + 1;
        int vx = (value - 1) % 3 + 1;

        parentObj.transform.Find($"y{vy}x{vx}").gameObject.SetActive(true);
        memoSudoku[value - 1, y, x] = true;
    }

    public void DeleteMemoCell(int y, int x, int value = 0)
    {
        GameObject[,] memoObjs = GetMemoObjects(y, x);
        if (value == 0) //Delete all
        {
            for (int _y = 0; _y < 3; _y++)
            {
                for (int _x = 0; _x < 3; _x++)
                {
                    memoObjs[_y, _x].SetActive(false);
                    memoSudoku[_y * 3 + _x, y, x] = false;
                }
            }
        }
        else //Delete one
        {
            int my = (value - 1) / 3;
            int mx = (value - 1) % 3;

            memoObjs[my, mx].SetActive(false);
            memoSudoku[value - 1, y, x] = false;
        }
    }

    public void DeleteMemoCellsAtOnce(int y, int x, int value) //입력 시 주변 메모셀 모두 정리
    {
        //row
        for (int _x = 0; _x < 9; _x++)
        {
            DeleteMemoCell(y, _x, value);
        }
        //col
        for (int _y = 0; _y < 9; _y++)
        {
            DeleteMemoCell(_y, x, value);
        }
        //SG
        for (int _y = y / 3 * 3; _y < y / 3 * 3 + 3; _y++)
        {
            for (int _x = x / 3 * 3; _x < x / 3 * 3 + 3; _x++)
            {
                DeleteMemoCell(_y, _x, value);
            }
        }
    }


    //메모 셀 처음 생성
    private void AppendMemoCellObjects()
    {
        var w = memoCell.transform.GetComponent<RectTransform>().sizeDelta.x;

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                GameObject parentObj = new GameObject("Memo");
                parentObj.transform.parent = objects[y, x].transform;
                parentObj.transform.localPosition = new Vector2(0, 0);

                //3x3 memo
                for (int my = 0; my < 3; my++)
                {
                    for (int mx = 0; mx < 3; mx++)
                    {
                        GameObject mCell = GameObject.Instantiate(memoCell, parentObj.transform);

                        Text tText = mCell.transform.Find("Text").GetComponent<Text>();
                        tText.text = (my * 3 + mx + 1).ToString();

                        mCell.transform.localPosition = new Vector2((mx - 1) * w, -1 * (my - 1) * w);
                        mCell.name = $"y{my + 1}x{mx + 1}";
                        mCell.SetActive(false);
                    }
                }
            }
        }
    }
}
