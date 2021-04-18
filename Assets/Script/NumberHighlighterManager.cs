using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberHighlighterManager : MonoBehaviour
{
    public GameObject numberHighlighter;
    public CellManager cellManager;

    private Button[] nhs;

    private void Start()
    {
        nhs = new Button[9];

        for (int index = 0; index < 9; index++)
        {
            string tString = $"button_{index + 1}";
            nhs[index] = gameObject.transform.Find(tString).GetComponent<Button>();

            int t = index + 1;
            nhs[index].onClick.AddListener(delegate { CallHighlightFunc(t); });
        }
    }

    private void CallHighlightFunc(int n)
    {
        cellManager.HighlightCells(n);
    }
}
