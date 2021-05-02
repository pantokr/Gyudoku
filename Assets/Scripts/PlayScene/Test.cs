using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private HintManager hintManager;
    private void Start()
    {
        hintManager = GameObject.Find("HintButton").GetComponent<HintManager>();
    }
    public void Tester()
    {

        hintManager.FindNakedSingle(false);
    }
}
