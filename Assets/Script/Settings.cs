using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static float Volume = 0.5f;
    public AudioSource Soruce;
    public static int Missing;
    public static Sprite Background;

    private void Update()
    {
        if(Soruce != null)
            Soruce.volume = Volume;
    }

    public void VolumeChange(Single volume)
    {
        Volume = volume;
    }
    
}
