using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintVisualManager : MonoBehaviour
{
    public CellManager cellManager;
    public MemoManager memoManager;
    public GameObject hintLine;

    private GameObject[,] objects;
    private List<GameObject> highlightedObjectList = new List<GameObject>();
    private List<GameObject> drawedLineList = new List<GameObject>();
    private List<(GameObject, GameObject)> gList = new List<(GameObject, GameObject)>();

    LineRenderer lr;
    private Color colorA;

    private void Start()
    {
        objects = cellManager.GetObjects();
        ColorUtility.TryParseHtmlString("#C7F6B6FF", out colorA);
    }
    private void Update()
    {
        for (int i = 0; i < drawedLineList.Count; i++)
        {
            var tlr = drawedLineList[i].GetComponent<LineRenderer>();

            Vector3 pos_g1 = gList[i].Item1.transform.position;
            Vector3 pos_g2 = gList[i].Item2.transform.position;

            pos_g1.z = 0;
            pos_g2.z = 0;

            tlr.SetPosition(0, pos_g1);
            tlr.SetPosition(1, pos_g2);
        }
    }

    public void DrawLine(GameObject g1, GameObject g2)
    {
        var line = Instantiate(hintLine, transform);
        drawedLineList.Add(line);

        LineRenderer lr;

        lr = line.GetComponent<LineRenderer>();
        lr.startWidth = 10f;
        lr.endWidth = 10f;

        Vector3 pos_g1 = g1.transform.position;
        Vector3 pos_g2 = g2.transform.position;

        pos_g1.z = 0;
        pos_g2.z = 0;

        lr.SetPosition(0, pos_g1);
        lr.SetPosition(1, pos_g2);

        gList.Add((g1, g2));
    }

    public void EraseAllLine()
    {
        foreach (var l in drawedLineList)
        {
            Destroy(l);
        }
        drawedLineList.Clear();
        gList.Clear();
    }

    public void HighlightCell(GameObject g)
    {
        g.transform.GetComponent<Image>().color = colorA;
        highlightedObjectList.Add(g);

        var objs = memoManager.GetActiveMemoObjects(g);
        if (objs == null)
        {
            return;
        }
        foreach (var obj in objs)
        {
            obj.transform.GetComponent<Image>().color = colorA;
            highlightedObjectList.Add(obj);
        }
    }

    public void EraseAllCell()
    {
        //강조 지우기
        foreach (var obj in highlightedObjectList)
        {
            if (obj.transform.Find("Memo") != null)
            {
                obj.transform.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                obj.transform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
        highlightedObjectList.Clear();
    }

    public void HighlightBundle(int code)
    {
        if (code >= 27 || code < 0)
        {
            return;
        }

        if (code < 9) // row
        {
            int r = code % 9;
            for (int _x = 0; _x < 9; _x++)
            {
                objects[r, _x].transform.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
                highlightedObjectList.Add(objects[r, _x]);

                var objs = memoManager.GetActiveMemoObjects(r, _x);
                foreach (var obj in objs)
                {
                    obj.transform.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                    highlightedObjectList.Add(obj);
                }
            }
        }
        else if (code < 18) //col
        {
            int c = code % 9;
            for (int _y = 0; _y < 9; _y++)
            {
                objects[_y, c].transform.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                highlightedObjectList.Add(objects[_y, c]);

                var objs = memoManager.GetActiveMemoObjects(_y, c);
                foreach (var obj in objs)
                {
                    obj.transform.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                    highlightedObjectList.Add(obj);
                }
            }
        }
        else
        {
            int sg = code % 9;

            int sy = sg / 3;
            int sx = sg % 3;
            for (int _y = sy * 3; _y < sy * 3 + 3; _y++)
            {
                for (int _x = sx * 3; _x < sx * 3 + 3; _x++)
                {
                    objects[_y, _x].transform.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                    highlightedObjectList.Add(objects[_y, _x]);

                    var objs = memoManager.GetActiveMemoObjects(_y, _x);
                    foreach (var obj in objs)
                    {
                        obj.transform.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                        highlightedObjectList.Add(obj);
                    }
                }
            }
        }
    }
}
