using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberHighlighterManager_C : MonoBehaviour
{
    public GameObject numberHighlighter;
    public CellManager_C cellManager;

    private Button[] nhs;

    private void Start()
    {
        nhs = new Button[9];

        for (int index = 0; index < 9; index++)
        {
            string tString = $"button_{index + 1}";
            nhs[index] = gameObject.transform.Find(tString).GetComponent<Button>();

            int t = index;
            nhs[index].onClick.AddListener(delegate { CallHighlightFunc(t + 1); });
        }
    }

    public void CallHighlightFunc(int n)
    {
        nhs[n - 1].Select();
        cellManager.HighlightCells(n);
    }
}
