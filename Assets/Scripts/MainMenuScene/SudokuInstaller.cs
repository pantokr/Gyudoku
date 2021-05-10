using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuInstaller : MonoBehaviour
{
    public FileManager_M fileManager;


    private void Start()
    {
        Settings.PlayMode = 3;

        BetterStreamingAssets.Initialize();

        bool easy_exist = BetterStreamingAssets.FileExists("/SudokuSample/easy.txt");
        bool medium_exist = BetterStreamingAssets.FileExists("/SudokuSample/medium.txt");
        bool hard_exist = BetterStreamingAssets.FileExists("/SudokuSample/hard.txt");

        if (!easy_exist || !medium_exist || !hard_exist)
        {
            Settings.PlayMode = 0;
            return;
        }

        string easy_all_text = BetterStreamingAssets.ReadAllText("/SudokuSample/easy.txt");
        string medium_all_text = BetterStreamingAssets.ReadAllText("/SudokuSample/medium.txt");
        string hard_all_text = BetterStreamingAssets.ReadAllText("/SudokuSample/hard.txt");

        string[] easy_maps = easy_all_text.Split('\n');
        string[] medium_maps = medium_all_text.Split('\n');
        string[] hard_maps = hard_all_text.Split('\n');

        int easy_cnt = 0;
        foreach (var map in easy_maps)
        {
            fileManager.StartSaving($"easy{easy_cnt++}", map);
        }
        Settings.Easy_Cnt = easy_cnt;

        int medium_cnt = 0;
        foreach (var map in medium_maps)
        {
            fileManager.StartSaving($"medium{medium_cnt++}", map);
        }
        Settings.Medium_Cnt = medium_cnt;

        int hard_cnt = 0;
        foreach (var map in hard_maps)
        {
            fileManager.StartSaving($"hard{hard_cnt++}", map);
        }
        Settings.Hard_Cnt = hard_cnt;

        print($"Installed {easy_cnt}, {medium_cnt}, {hard_cnt}");

    }
}
