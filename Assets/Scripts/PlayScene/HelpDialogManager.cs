using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpDialogManager : MonoBehaviour
{
    public GameObject helpButton;
    public GameObject manualTools;
    public GameObject numberHighlighters;
    public GameObject autoTools;
    public GameObject fileTools;

    private string[] texts;

    private Text _dialogText;
    private int cur = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeText();
        }
    }

    // hint button >> hint manager >> dialog
    public void StartDialog()
    {
        helpButton.SetActive(false);
        _dialogText = transform.Find("DialogText").GetComponent<Text>();

        string[] _texts = { "스도쿠를 더 쉽게 풀 수 있도록 해주는 도구들이 있습니다.",
            "1. 메모 : 'Tab'을 누르면 메모 모드를 켜고 끌 수 있으며(화면 색이 변합니다), 셀에서 'Shift + 키보드의 숫자'를 누르면 빠르게 메모할 수 있습니다.)",
            "2. 지우개 : 'E'를 누르면 지우개 모드를 켜고 끌 수 있으며(화면 색이 변합니다), 지우개 모드 상태에서 셀을 클릭하면 셀의 데이터가 지워집니다. 셀을 선택한 상태에서 'E'를 누르면 빠르게 지울 수 있습니다.",
            "3. 되돌리기 : 'Q'를 누르면 직전의 스도쿠로 되돌립니다. 규민이가 이런 것도 만들줄 압니다.",
            "4. 숫자 강조 : 이 버튼 중 하나를 누르면 해당하는 숫자를 강조합니다. 'Ctrl + 키보드의 숫자'를 누르면 숫자 강조 버튼을 누르지 않고 바로 숫자를 강조할 수 있습니다.",
            "5. 자동 메모 : 모든 셀에 메모를 자동으로 채워줍니다.",
            "6. !힌트! : 스도쿠를 한 단계씩 자동으로 풀어줍니다.",
            "7. 자동 싱글 : 스도쿠 풀이에서 가장 기본적인 '나머지 한 칸 채우기'를 자동으로 해결합니다.",
            "8. 스도쿠 저장 : 파일명을 지정해 저장하면, 메인 메뉴의 커스텀 모드에서 다시 플레이 할 수 있습니다."};

        texts = (string[])_texts.Clone();

        //대화 상자 켜기
        gameObject.SetActive(true);

        cur = 0; //현재 진행 상황 갱신
        ChangeText();
    }
    public void ChangeText()
    {
        if (cur == texts.Length)
        {
            //대사화면 정리
            helpButton.SetActive(true);
            gameObject.SetActive(false);

            manualTools.SetActive(true);
            numberHighlighters.SetActive(true);
            autoTools.SetActive(true);

            return;
        }
        //text 변경
        _dialogText.text = texts[cur];

        if (cur == 0)
        {
            numberHighlighters.SetActive(false);
            autoTools.SetActive(false);
            fileTools.SetActive(false);
        }

        if (cur == 4)
        {
            numberHighlighters.SetActive(true);
            manualTools.SetActive(false);
        }

        if (cur == 5)
        {
            numberHighlighters.SetActive(false);
            autoTools.SetActive(true);
        }

        if(cur == 8)
        {
            fileTools.SetActive(true);
            autoTools.SetActive(false);
        }


        cur++;
    }

    void SetActive(bool onf)
    {

    }
}


