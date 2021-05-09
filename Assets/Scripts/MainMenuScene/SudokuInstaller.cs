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

        string easy_filepath = Path.Combine(Application.dataPath + "/SudokuSample/easy.txt");
        string medium_filepath = Path.Combine(Application.dataPath + "/SudokuSample/medium.txt");
        string hard_filepath = Path.Combine(Application.dataPath + "/SudokuSample/hard.txt");

        FileInfo fileinfo_easy = new FileInfo(easy_filepath);
        FileInfo fileinfo_medium = new FileInfo(medium_filepath);
        FileInfo fileinfo_hard = new FileInfo(hard_filepath);

        StreamReader file_easy = new StreamReader(easy_filepath);
        StreamReader file_medium = new StreamReader(medium_filepath);
        StreamReader file_hard = new StreamReader(hard_filepath);

        if (!fileinfo_easy.Exists || !fileinfo_medium.Exists || !fileinfo_hard.Exists)
        {
            Settings.PlayMode = 0;
        }

        int cnt_easy = 0;
        string line_e;
        while ((line_e = file_easy.ReadLine()) != null)
        {
            fileManager.StartSaving($"easy{cnt_easy++}", line_e);
        }
        Settings.Easy_Cnt = cnt_easy;
        
        int cnt_medium = 0;
        string line_m;
        while ((line_m = file_medium.ReadLine()) != null)
        {
            fileManager.StartSaving($"medium{cnt_medium++}", line_m);
        }
        Settings.Medium_Cnt = cnt_medium;
        
        int cnt_hard = 0;
        string line_h;
        while ((line_h = file_hard.ReadLine()) != null)
        {
            fileManager.StartSaving($"hard{cnt_hard++}", line_h);
        }
        Settings.Hard_Cnt = cnt_hard;
    }
}
