using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySetter : MonoBehaviour
{
    // Start is called before the first frame update
    public static int EmptyC1 = 0;
    public static int EmptyC2 = 0;
    public static int EmptyMiddle = 1;
    public static int PatternCode = 0;

    public static int Difficulty = 1;

    // Update is called once per frame
    public static void SetEasyMode()
    {
        EmptyC1 = Random.Range(4, 7);
        EmptyC2 = 10 - EmptyC1;
        EmptyMiddle = Random.Range(2, 4);
        PatternCode = 0;
        Difficulty = 0;
    }
    public static void SetMediumMode()
    {
        EmptyC1 = Random.Range(5, 7);
        EmptyC2 = 11 - EmptyC1;
        EmptyMiddle = Random.Range(2, 6);
        PatternCode = Random.Range(0, 2);
        Difficulty = 1;
    }
    public static void SetHardMode()
    {

        EmptyC1 = Random.Range(5, 8);
        EmptyC2 = 12 - EmptyC1;
        EmptyMiddle = 5;
        PatternCode = Random.Range(0, 2);
        Difficulty = 2;
    }

}
