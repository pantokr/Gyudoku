using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintLineManager : MonoBehaviour
{
    public GameObject hintLine;
    public GameObject row;
    public GameObject col;

    private int n_line = 0;
    private List<GameObject> highlightedObjectList = new List<GameObject>();

    private void Start()
    {
        //DrawLine(GameObject.Find("R2C1"), GameObject.Find("R7C1"));
    }
    public void DrawLine(GameObject g1, GameObject g2)
    {

        Vector3 pos_g1 = g1.transform.position;
        Vector3 pos_g2 = g2.transform.position;

        if (pos_g1.x == pos_g2.x) //세로선 
        {
            GameObject line = Instantiate(col, hintLine.transform);
            line.name = $"line{++n_line}";

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
            g1.transform.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
            g2.transform.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
            highlightedObjectList.Add(g1);
            highlightedObjectList.Add(g2);
        }
        else if (pos_g1.y == pos_g2.y) //가로선
        {

            GameObject line = Instantiate(row, hintLine.transform);
            line.name = $"line{++n_line}";

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
            g1.transform.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
            g2.transform.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
            highlightedObjectList.Add(g1);
            highlightedObjectList.Add(g2);
        }
        else
        {
            print("HINT LINE ERROR");
            return;
        }
    }
    public void EraseAllLine()
    {
        for (int child = 0; child < hintLine.transform.childCount; child++)
        {
            GameObject obj = hintLine.transform.GetChild(child).gameObject;
            Destroy(obj);
        }
        n_line = 0;

        //강조 지우기
        foreach(var obj in highlightedObjectList)
        {
            obj.transform.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        highlightedObjectList.Clear();
    }

    public void HighlightCell(GameObject g)
    {
        g.transform.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
        highlightedObjectList.Add(g);
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
}
