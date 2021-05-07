using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoManager : MonoBehaviour
{
    //FB92C4 128
    public GameObject memoCell;
    public CellManager cellManager;

    public int[,,] memoSudoku;
    private GameObject[,] objects;

    private void Start()
    {
        objects = cellManager.GetObjects();
        AppendMemoCellObjects();
        memoSudoku = SudokuManager.memoSudoku;
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
        GameObject[,] retObj = new GameObject[3, 3];
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
    public List<GameObject> GetActiveMemoObjects(int y, int x)
    {
        List<GameObject> list = new List<GameObject>();
        var objs = GetMemoObjects(y, x);
        foreach (var obj in objs)
        {
            if (obj.activeSelf == true)
            {
                list.Add(obj);
            }
        }
        return list;
    }

    public List<GameObject> GetActiveMemoObjects(GameObject parent)
    {
        List<GameObject> list = new List<GameObject>();
        if (parent.transform.Find("Memo") == null)
        {
            return null;
        }
        var name = parent.name;
        int y = name[1] - '0' - 1;
        int x = name[3] - '0' - 1;

        var objs = GetMemoObjects(y, x);
        foreach (var obj in objs)
        {
            if (obj.activeSelf == true)
            {
                list.Add(obj);
            }
        }
        return list;
    }
    public GameObject[,,,] GetWholeMemoObjects() //(y,x)의 (3x3 메모스도쿠)
    {
        GameObject[,,,] retObj = new GameObject[9, 9, 3, 3];
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int _y = 0; _y < 3; _y++)
                {
                    for (int _x = 0; _x < 3; _x++)
                    {
                        retObj[y, x, _y, _x] = GetMemoObject(y, x, _y * 3 + _x + 1);
                    }
                }
            }
        }
        return retObj;
    }
    public void SetMemoSudoku(int[,,] ms)
    {
        this.memoSudoku = (int[,,])ms.Clone();
    }
    public void ApplyMemoSudoku(int[,,] memoSudoku)
    {
        for (int val = 0; val < 9; val++)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (memoSudoku[val, y, x] == 0)
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
        memoSudoku[value - 1, y, x] = 1;
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
                    memoSudoku[_y * 3 + _x, y, x] = 0;
                }
            }
        }
        else //Delete one
        {
            int my = (value - 1) / 3;
            int mx = (value - 1) % 3;

            memoObjs[my, mx].SetActive(false);
            memoSudoku[value - 1, y, x] = 0;
        }
    }
    public void DeleteMemoCell(GameObject obj)
    {
        obj.SetActive(false);
        int value = int.Parse(obj.transform.Find("Text").GetComponent<Text>().text);

        GameObject parent = obj.transform.parent.parent.gameObject;
        string parentName = parent.name;
        int y = parentName[1] - '0' - 1;
        int x = parentName[3] - '0' - 1;
        memoSudoku[value - 1, y, x] = 0;
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
                        GameObject mCell = Instantiate(memoCell, parentObj.transform);

                        Text tText = mCell.transform.Find("Text").GetComponent<Text>();
                        tText.text = (my * 3 + mx + 1).ToString();

                        Image tImage = mCell.GetComponent<Image>();
                        tImage.sprite = null;

                        mCell.transform.localPosition = new Vector2((mx - 1) * w, -1 * (my - 1) * w);
                        mCell.name = $"y{my + 1}x{mx + 1}";
                        mCell.SetActive(false);
                    }
                }
            }
        }
    }
}
