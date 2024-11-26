using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    Text txt_Dialogue;
    [SerializeField]
    Text txt_Name;
    [SerializeField]
    private GameObject choicePanel; // 선택지 UI 패널
    [SerializeField]
    private Button choicePrefab; // 선택지 버튼 프리팹
    [SerializeField]
    private float buttonSpacing = 60f; // 선택지 버튼 간격
    [SerializeField]
    private float startY = 100f; // 선택지 버튼 y 좌표
    [SerializeField]
    private float startX = 0f; // 선택지 버튼 x 좌표
    [SerializeField]
    private Vector2 buttonSize = new Vector2(600f, 100f); // 선택지 버튼 크기


    public Dialogue[] dialogues;

    bool isDialogue = false; // 현재 대화중인지
    bool isNext = false; // 다음 대사 대기
    bool isChoice = false; // 선택 중이 아닐때
    bool isFinish = false; // 대화 종료 여부

    int lineCount = 0; // 대화 카운트
    int contextCount = 0; // 대사 카운트

    [Header("텍스트 출력 딜레이")]
    [SerializeField] float textDelay;

    private void Update() {
        ShowDialouge();
    }

    private void ShowDialouge()
    {
                if(!isDialogue || !isNext || !Input.GetKeyDown(KeyCode.Space)) return;
        
        isNext = false;
        txt_Dialogue.text = "";
        
        // 현재 대화가 종료 표시된 경우
        if(isFinish && dialogues[lineCount].isEnd)
        {
            EndDialogue();
            return;
        }
        
        // 현재 대화의 다음 문장이 있는 경우
        if(contextCount + 1 < dialogues[lineCount].contexts.Length) 
        {
            contextCount++;
            StartCoroutine(TypeWriter());
            return;
        }
        
        // 현재 대화가 끝난 경우
        contextCount = 0;
        
        // 다음 대사 번호가 지정된 경우
        if(dialogues[lineCount]._nextDialogueIndex > 0)
        {
            lineCount = dialogues[lineCount]._nextDialogueIndex - 1;
            if(lineCount < dialogues.Length)
            {
                StartCoroutine(TypeWriter());
                return;
            }
        }
        // 다음 순차적인 대화가 있는 경우
        else if(lineCount + 1 < dialogues.Length)
        {
            lineCount++;
            StartCoroutine(TypeWriter());
            return;
        }
        
        // 위의 조건들을 만족하지 않으면 대화 종료
        EndDialogue();
    }

    public void ShowChoices(DialogueChoice[] choices) // 선택지 표시
    {
        if(choices == null || choices.Length == 0) return; // 선택지가 없으면 return

        isChoice = true;
        choicePanel.SetActive(true);  // 선택지 패널 활성화

        for(int i = 0; i < choices.Length; i++) // 선택지 개수만큼 반복
        {
            Button choiceButton = Instantiate(choicePrefab, choicePanel.transform); // 선택지 버튼 생성
            RectTransform rectTransform = choiceButton.GetComponent<RectTransform>(); // 선택지 버튼 위치 설정
            rectTransform.anchoredPosition = new Vector2(startX, startY - (buttonSpacing * i)); // 선택지 버튼 위치 설정
            rectTransform.sizeDelta = buttonSize; 

            DialogueChoiceButton choiceButtonComponent = choiceButton.GetComponent<DialogueChoiceButton>();
            choiceButtonComponent.Setup(choices[i], OnChoiceSelected); // 
        }
    }


    public void OnChoiceSelected(DialogueChoice choice) // 선택지 선택 시
    {
        choicePanel.SetActive(false);
        ClearChoices();
        isChoice = false;
        if (choice.nextDialogueIndex >= 0 && choice.nextDialogueIndex <= dialogues.Length)
        {
            lineCount = choice.nextDialogueIndex - 1;
            contextCount = 0;
            txt_Dialogue.text = "";
            txt_Name.text = "";
            
            StartCoroutine(TypeWriter());
        }
        else
        {
            Debug.LogError($"Invalid dialogue index: {choice.nextDialogueIndex}");
            EndDialogue();
        }
    }

    private void ClearChoices() // 선택지 초기화
    {
        foreach(Transform child in choicePanel.transform) // 선택지 패널 자식 오브젝트 초기화
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowDialogue(Dialogue[] _Dialogues)
    {
        isDialogue = true;
        txt_Dialogue.text = "";
        txt_Name.text = "";
        dialogues = _Dialogues;
        StartCoroutine(TypeWriter());
    }

    void EndDialogue()
    {
        isDialogue = false;
        lineCount = 0;
        contextCount = 0;
        dialogues = null;
        isNext = false;
        HideUI();
        Debug.Log("대화 종료");
        isFinish = true;
    }


    IEnumerator TypeWriter() // 타자 효과 시작
    {
        string t_ReplaceText = dialogues[lineCount].contexts[contextCount]; // 대사 가져오기
        t_ReplaceText = t_ReplaceText.Replace("'", ","); // 대사에서 쉼표(,)로 대체

        txt_Name.text = dialogues[lineCount].name; // 이름 표시
        txt_Dialogue.text = ""; // 대사 초기화
        isFinish = false; // 대사 종료 플래그 초기화

        for(int i = 0; i < t_ReplaceText.Length; i++) // 대사 길이만큼 반복
        {
            txt_Dialogue.text += t_ReplaceText[i]; // 대사 표시
            yield return new WaitForSeconds(textDelay);
        }
        
        isFinish = true; // 대사 종료

        // 선택지가 존재하고 대사가 끝나면 선택지 표시
        if(dialogues != null && 
            lineCount < dialogues.Length && 
            dialogues[lineCount] != null && 
            dialogues[lineCount].choices != null && 
            dialogues[lineCount].choices.Length > 0)
        {
            ShowChoices(dialogues[lineCount].choices);
        }
        else
        {
            isNext = true;
        }
    }

    void HideUI()
    {
        txt_Dialogue.text = "";
        txt_Name.text = "";
    }

}
