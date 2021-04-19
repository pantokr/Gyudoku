using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static float Volume = 0.5f;
    public AudioSource Source;

    public static int EmptyC1 = 3;
    public static int EmptyC2 = 3;
    public static int EmptyMiddle = 3;
    public static int PatternCode = 0;

    public static int PlayMode = 0;
    // 0 default
    // 1 new
    // 2 open

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
