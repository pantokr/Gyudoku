using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FileManager_M : MonoBehaviour
{
    public static string fname = "default";

    public GameObject panel;
    public GameObject dialogText;
    public InputField inputField;
    public GameObject cancelButton;
    public GameObject submitButton;

    private Button _open;
    private Button _cancel;
    private Button _submit;

    public int[,] cache = new int[9, 9];

    private void Start()
    {
        _open = gameObject.GetComponent<Button>();
        _open.onClick.AddListener(delegate { DisplayDialog(); });

        _cancel = cancelButton.GetComponent<Button>();
        _cancel.onClick.AddListener(delegate { Cancel(); });

        _submit = submitButton.GetComponent<Button>();
        _submit.onClick.AddListener(delegate { Submit(); });
    }

    public void StartSaving(string name, string str)
    {
        PlayerPrefs.SetString(name, str);
        //print("Saved");
    }

    private void StartSaving(string name = "default")
    {

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

    private void StartOpening(string name = "default")
    {

        string str2arr = PlayerPrefs.GetString(name);
        //print(str2arr);

        int index = 0;
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                cache[y, x] = str2arr[index++] - '0';
            }
        }
        print("Opened");
    }

    private void DisplayDialog()
    {
        for (int child = 0; child < panel.transform.childCount; child++)
        {
            GameObject obj = panel.transform.GetChild(child).gameObject;
            if (obj.name == "FileNameSetter")
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }
    private void Cancel()
    {
        for (int child = 0; child < panel.transform.childCount; child++)
        {
            GameObject obj = panel.transform.GetChild(child).gameObject;
            if (obj.name == "FileNameSetter" || obj.name == "Customized")
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
        }
    }

    private void Submit()
    {
        Text _fname = inputField.transform.Find("Text").GetComponent<Text>();
        fname = _fname.text;

        if (!PlayerPrefs.HasKey(fname))
        {
            print("No key");
            dialogText.GetComponent<Text>().text = "No such file";
            return;
        }

        StartOpening(fname);
        StartSaving();

        SceneManager.LoadScene("LoadingScene");
    }
}

