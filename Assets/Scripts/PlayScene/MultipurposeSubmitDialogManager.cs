using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipurposeSubmitDialogManager : MonoBehaviour
{
    Text text;
    public void RunDialog(string str)
    {
        gameObject.SetActive(true);
        text = transform.Find("DialogText").GetComponent<Text>();

        text.text = str;
        StartCoroutine(Vanisher());
    }

    private IEnumerator Vanisher()
    {
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }
}
