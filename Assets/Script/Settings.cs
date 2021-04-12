using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static float Volume = 0.5f;
    public AudioSource Soruce;

    public static int EmptyC1 = 3;
    public static int EmptyC2 = 3;
    public static int EmptyMiddle = 3;
    public static int PatternCode = 0;

    public static Sprite Background;

    private void Update()
    {
        if (Soruce != null)
            Soruce.volume = Volume;
    }

    public void VolumeChange(Single volume)
    {
        Volume = volume;
    }

}
