using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoToolsManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject autoMemoButton;
    public GameObject hintButton;
    public GameObject autoSingleButton;
    public HintDialogManager hintDialogManager;

    private AutoMemoManager autoMemoManager;
    private HintManager hintManager;
    private AutoSingleManager autoSingleManager;

    private Button _autoMemo;
    private Button _hint;
    private Button _autoSingle;

    private void Start()
    {
        if (DifficultySetter.Difficulty == 0)
        {
            autoMemoButton.SetActive(false);
            hintButton.SetActive(false);
            autoSingleButton.SetActive(false);
        }

        autoMemoManager = autoMemoButton.GetComponent<AutoMemoManager>();
        hintManager = hintButton.GetComponent<HintManager>();
        autoSingleManager = autoSingleButton.GetComponent<AutoSingleManager>();

        _autoMemo = autoMemoButton.GetComponent<Button>();
        _hint = hintButton.GetComponent<Button>();
        _autoSingle = autoSingleButton.GetComponent<Button>();

        _autoMemo.onClick.AddListener(delegate { autoMemoManager.RunAutoMemo(); });
        _hint.onClick.AddListener(delegate { hintManager.RunHint(); });

        _autoSingle.onClick.AddListener(delegate { autoSingleManager.RunAutoSingle(); });
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.H))
        {
            hintManager.RunHint();
        }
    }
}
