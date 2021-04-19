using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinisherManager : MonoBehaviour
{
    public CellManager cellManager;

    public GameObject saveButton;
    private Button _save;
    private void Start()
    {
        _save = saveButton.GetComponent<Button>();
        _save.onClick.AddListener(delegate { StartSaving("Sudoku"); });
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
                arr2str += cache[y, x].ToString();
            }
        }

        PlayerPrefs.SetString(name, arr2str); // PlyerPrefs�� ���ڿ� ���·� ����
        //print(arr2str);
        print("Saved");
    }

    public void StartOpening(string name = "default")
    {
        int[,] cache = new int[9, 9]; // ������ �迭 ����
        string str2arr = PlayerPrefs.GetString(name);

        int index = 0;
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                cache[y, x] = str2arr[index++] - '0';
            }
        }
        cellManager.SetSudoku(cache);
        print("Opened");
    }
}
