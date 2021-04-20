using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoManager : MonoBehaviour
{
    //FB92C4 128
    public GameObject memoCell;
    public CellManager cellManager;

    private GameObject[,] objects;

    private void Start()
    {
        objects = cellManager.GetObjects();

        ApplyMemoCells();
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
    public void FillMemoCell(int y, int x, int value)
    {
        GameObject parentObj = objects[y, x].transform.Find("Memo").gameObject;

        int vy = (value - 1) / 3 + 1;
        int vx = (value - 1) % 3 + 1;

        parentObj.transform.Find($"y{vy}x{vx}").gameObject.SetActive(true);
    }

    public void DeleteMemoCell(int y, int x, int value = 0)
    {
        GameObject[,] memoObjs = GetMemoObjects(y, x);
        if (value == 0)
        {
            for (int _y = 0; _y < 3; _y++)
            {
                for (int _x = 0; _x < 3; _x++)
                {
                    memoObjs[_y, _x].SetActive(false);
                }
            }
        }
        else
        {
            int my = (value - 1) / 3;
            int mx = (value - 1) % 3;
            memoObjs[my, mx].SetActive(false);
        }
    }

    private void ApplyMemoCells()
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
