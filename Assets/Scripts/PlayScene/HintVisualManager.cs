using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintVisualManager : MonoBehaviour
{
    public GameObject hintLine;
    public GameObject row;
    public GameObject col;

    public CellManager cellManager;
    public MemoManager memoManager;

    private GameObject[,] objects;
    private List<GameObject> LineObjectList = new List<GameObject>();
    private List<GameObject> highlightedObjectList = new List<GameObject>();

    private Color colorA;

    private void Start()
    {
        objects = cellManager.GetObjects();

        ColorUtility.TryParseHtmlString("#C7F6B6FF", out colorA);
    }
    public void DrawLine(GameObject g1, GameObject g2)
    {

        Vector3 pos_g1 = g1.transform.position;
        Vector3 pos_g2 = g2.transform.position;

        if (pos_g1.x == pos_g2.x) //세로선 
        {
            GameObject line = Instantiate(col, hintLine.transform);
            line.name = $"line{LineObjectList.Count}";

            RectTransform rect_col = line.GetComponent<RectTransform>();
            RectTransform rect_g1 = g1.GetComponent<RectTransform>();

            float M = pos_g1.y > pos_g2.y ? pos_g1.y : pos_g2.y;
            float m = pos_g1.y < pos_g2.y ? pos_g1.y : pos_g2.y;


            M -= rect_g1.sizeDelta.y;
            m += rect_g1.sizeDelta.y;

            rect_col.position = new Vector3(pos_g1.x, rect_col.position.y);
            rect_col.offsetMax = new Vector2(rect_col.offsetMax.x, M); //top
            rect_col.offsetMin = new Vector2(rect_col.offsetMin.x, m); //bottom


            //강조
            g1.transform.GetComponent<Image>().color = new Color(0, 1, 0, 1.0f);
            g2.transform.GetComponent<Image>().color = new Color(0, 1, 0, 1.0f);

            highlightedObjectList.Add(g1);
            highlightedObjectList.Add(g2);

            LineObjectList.Add(line);
        }
        else if (pos_g1.y == pos_g2.y) //가로선
        {

            GameObject line = Instantiate(row, hintLine.transform);
            line.name = $"line{LineObjectList.Count}";

            RectTransform rect_row = line.GetComponent<RectTransform>();
            RectTransform rect_g1 = g1.GetComponent<RectTransform>();

            float M = pos_g1.x > pos_g2.x ? pos_g1.x : pos_g2.x;
            float m = pos_g1.x < pos_g2.x ? pos_g1.x : pos_g2.x;

            M -= rect_g1.sizeDelta.x;
            m += rect_g1.sizeDelta.x;

            rect_row.position = new Vector3(rect_row.position.x, pos_g1.y);
            rect_row.offsetMax = new Vector2(M, rect_row.offsetMax.y); //left
            rect_row.offsetMin = new Vector2(m, rect_row.offsetMin.y); //right

            //강조
            g1.transform.GetComponent<Image>().color = new Color(0, 1, 0, 1.0f);
            g2.transform.GetComponent<Image>().color = new Color(0, 1, 0, 1.0f);

            highlightedObjectList.Add(g1);
            highlightedObjectList.Add(g2);
            LineObjectList.Add(line);
        }
        else
        {
            print("HINT LINE ERROR");
            return;
        }
    }

    public void EraseAllLine()
    {
        foreach (var obj in LineObjectList)
        {
            Destroy(obj);
        }
        LineObjectList.Clear();

        //강조 지우기
        foreach (var obj in highlightedObjectList)
        {
            obj.transform.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        highlightedObjectList.Clear();
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
            obj.transform.GetComponent<Image>().color = new Color(1, 1, 1, 1);
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
