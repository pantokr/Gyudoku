using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinisherManager_C : MonoBehaviour
{
    public static string fname = "Sudoku";
    public CellManager_C cellManager;
   
    public GameObject saveButton;
    private Button _save;
    
    public GameObject playButton;
    private Button _play;
    private void Start()
    {
        _save = saveButton.GetComponent<Button>();
        _save.onClick.AddListener(delegate { StartSaving(fname); });
    
        _play = playButton.GetComponent<Button>();
        _play.onClick.AddListener(delegate { StartPlaying(); });
    }

    public void StartSaving(string name = "default")
    {
        int[,] cache; // ������ �迭 ����

        cache = (int[,])cellManager.GetSudokuValues().Clone();

        string arr2str = ""; // ���ڿ� ����

        for (int y = 0; y < 9; y++) // �迭�� ','�� �����ư��� tempStr�� ����
        {
            for (int x = 0; x < 9; x++)
            {
                arr2str += cache[y,x].ToString();
            }
        }

        PlayerPrefs.SetString(name, arr2str); // PlyerPrefs�� ���ڿ� ���·� ����
        //print(arr2str);
        print("Saved");
    }

    public void StartPlaying()
    {
        StartSaving();
        SceneManager.LoadScene("PlayScene");
    }
}
