using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FileManager_C : MonoBehaviour
{
    public static string fname = "default";
    public CellManager_C cellManager;
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
        StartSaving();
        SceneManager.LoadScene("PlayScene");
    }

    public void DisplayDialog()
    {
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
}
