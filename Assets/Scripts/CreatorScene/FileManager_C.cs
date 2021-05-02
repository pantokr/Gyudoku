using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FileManager_C : MonoBehaviour
{
    public static string fname = "default";
    public CellManager_C cellManager;
    public MultipurposeDialogManager_C multipurposeDialogManager;
    public GameObject playManager;
    public GameObject saveButton;
    public GameObject playButton;
    public GameObject dialog;
    public GameObject cancelButton;
    public GameObject submitButton;
    public GameObject mainPanel;
    public InputField inputField;

    private Button _save;
    private Button _play;
    private Button _cancel;
    private Button _submit;
    private void Start()
    {
        _save = saveButton.GetComponent<Button>();
        _save.onClick.AddListener(delegate { DisplayDialog(); });

        _play = playButton.GetComponent<Button>();
        _play.onClick.AddListener(delegate { StartPlaying(); });

        _cancel = cancelButton.GetComponent<Button>();
        _cancel.onClick.AddListener(delegate { Cancel(); });

        _submit = submitButton.GetComponent<Button>();
        _submit.onClick.AddListener(delegate { Submit(); });
    }
    public void StartSaving(string name = "default")
    {
        

        int[,] cache; // 정수형 배열 생성

        cache = (int[,])cellManager.GetSudokuValues().Clone();

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
    public void StartPlaying()
    {
        if (!IsNormalSudoku())
        {
            multipurposeDialogManager.RunDialog("오류를 모두 수정해주세요.");
            return;
        }
        StartSaving();
        SceneManager.LoadScene("PlayScene");
    }

    public void DisplayDialog()
    {
        if (!IsNormalSudoku())
        {
            multipurposeDialogManager.RunDialog("오류를 모두 수정해주세요.");
            return;
        }

        playManager.SetActive(false);
        dialog.SetActive(true);
        TurnObjects(false);
    }


    private void Cancel()
    {
        dialog.SetActive(false);
        playManager.SetActive(true);
        TurnObjects(true);
    }

    private void Submit()
    {
        Text _fname = inputField.transform.Find("Text").GetComponent<Text>();
        fname = _fname.text;

        StartSaving(fname);

        dialog.SetActive(false);
        playManager.SetActive(true);
        TurnObjects(true);
    }
    private void TurnObjects(bool onf)
    {
        playManager.SetActive(onf);
        mainPanel.transform.Find("SudokuBoard").gameObject.SetActive(onf);
        mainPanel.transform.Find("NumberHighlighter").gameObject.SetActive(onf);
        mainPanel.transform.Find("ManualTools").gameObject.SetActive(onf);
    }

    public bool IsNormalRow(int y)
    {
        List<int> nums = new List<int>();
        for (int _x = 0; _x < 9; _x++)
        {
            int val = cellManager.sudoku[y, _x];
            if (val != 0 && nums.Contains(val))
            {
                return false;
            }
            else
            {
                nums.Add(val);
            }
        }
        return true;
    }
    public bool IsNormalCol(int x)
    {
        List<int> nums = new List<int>();
        for (int _y = 0; _y < 9; _y++)
        {
            int val = cellManager.sudoku[_y, x];
            if (val != 0 && nums.Contains(val))
            {
                return false;
            }
            else
            {
                nums.Add(val);
            }
        }
        return true;
    }
    public bool IsNormalSG(int y, int x)
    {
        List<int> nums = new List<int>();
        for (int _y = y * 3; _y < y * 3 + 3; _y++)
        {
            for (int _x = x * 3; _x < x * 3 + 3; _x++)
            {
                int val = cellManager.sudoku[_y, _x];
                if (val != 0 && nums.Contains(val))
                {
                    return false;
                }
                else
                {
                    nums.Add(val);
                }
            }
        }
        return true;
    }

    public bool IsNormalSudoku()
    {
        for (int y = 0; y < 9; y++)
        {
            if (!IsNormalRow(y))
            {
                return false;
            }
        }

        for (int x = 0; x < 9; x++)
        {
            if (!IsNormalCol(x))
            {
                return false;
            }
        }
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (!IsNormalSG(y, x))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
