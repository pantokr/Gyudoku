using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
    public static string fname = "default";
    public CellManager cellManager;
    public GameObject playManager;

    public GameObject saveButton;

    public GameObject dialog;
    public InputField inputField;
    public GameObject cancelButton;
    public GameObject submitButton;

    private Button _save;
    private Button _cancel;
    private Button _submit;

    public int[,] cache =  new int[9, 9];

    private void Start()
    {
        _save = saveButton.GetComponent<Button>();
        _save.onClick.AddListener(delegate { DisplayDialog(); });

        _cancel = cancelButton.GetComponent<Button>();
        _cancel.onClick.AddListener(delegate { Cancel(); });

        _submit = submitButton.GetComponent<Button>();
        _submit.onClick.AddListener(delegate { Submit(); });
    }

    public void StartSaving(string name = "default")
    {
        int[,] cache; // 정수형 배열 생성

        cache = (int[,])SudokuManager.sudoku.Clone();

        string arr2str = ""; // 문자열 생성

        for (int y = 0; y < 9; y++) // 배열과 ','를 번갈아가며 tempStr에 저장
        {
            for (int x = 0; x < 9; x++)
            {
                arr2str += cache[y, x].ToString();
            }
        }

        PlayerPrefs.SetString(name, arr2str); // PlyerPrefs에 문자열 형태로 저장
        //print(arr2str);
        print("Saved");
    }
    public void StartOpening(string name = "default")
    {
        int[,] cache = new int[9, 9];
        string str2arr = PlayerPrefs.GetString(name);

        int index = 0;
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                cache[y, x] = str2arr[index++] - '0';
            }
        }
        SudokuManager.sudoku = (int[,])cache.Clone();
        print("Opened");
    }

    private void DisplayDialog()
    {
        GameObject.Find("PlayManager").SetActive(false);
        dialog.SetActive(true);
    }
    private void Cancel()
    {
        dialog.SetActive(false);
        playManager.SetActive(true);
    }

    private void Submit()
    {
        Text _fname = inputField.transform.Find("Text").GetComponent<Text>();
        fname = _fname.text;

        StartSaving(fname);

        dialog.SetActive(false);
        playManager.SetActive(true);
    }
}

