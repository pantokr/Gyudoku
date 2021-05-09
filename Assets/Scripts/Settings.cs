using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static float Volume = 0.5f;
    public AudioSource Source;

    public static int PlayMode = 0;
    // 0 default
    // 1 new
    // 2 open
    // 3 all customized

    public static int Easy_Cnt = 0;
    public static int Medium_Cnt = 0;
    public static int Hard_Cnt = 0;

    private void Update()
    {
        if (Source != null)
            Source.volume = Volume;
    }
    public void ChangeVolume(Single volume)
    {
        Volume = volume;
    }


}
