using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    [SerializeField]
    private IllustrationManager illustrationManager; // IllustrationManager 참조 추가


    public Dialogue[] dialogues;

    bool isDialogue = false; // 현재 대화중인지
    bool isNext = false; // 다음 대사 대기
    bool isChoice = false; // 선택 중이 아닐때
    bool isFinish = false; // 대화 종료 여부

    int lineCount = 0; // 대화 카운트
    int contextCount = 0; // 대사 카운트
    int flagCount = 0; // 플래그 카운트
    
    string currentFlag = ""; // 현재 플래그

    [Header("텍스트 출력 딜레이")]
    [SerializeField] float textDelay;

    private void Start() {
        illustrationManager = FindObjectOfType<IllustrationManager>();
        if(illustrationManager == null)
        {
            Debug.LogError("IllustrationManager 찾을 수 없음");
        }
    }

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
        
        // 한 캐릭터가 대사를 두개 이상 칠 때
        // 엔터 누르고 ID 비워두고 대사
        if(contextCount + 1 < dialogues[lineCount].contexts.Length) 
        {
            contextCount++;
            StartCoroutine(TypeWriter());
            return;
        }
        
        // 현재 대화가 끝난 경우
        if(isFinish)
        {
            contextCount = 0;
        }
        
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
            choiceButtonComponent.Setup(choices[i], OnChoiceSelected); 
            Debug.Log(choices[i].flag + "플래그 버튼에 추가");
        }
    }


    public void OnChoiceSelected(DialogueChoice choice) // 선택지 선택 시
    {
        choicePanel.SetActive(false);
        ClearChoices();
        isChoice = false;

        if (illustrationManager != null)
        {
            illustrationManager.ChangeIllustration(choice.illustrationIndex);
        }

        // 선택지 인덱스가 유효한 경우
        if (choice.nextDialogueIndex >= 0 && choice.nextDialogueIndex <= dialogues.Length)
        {
            lineCount = choice.nextDialogueIndex - 1;
            contextCount = 0;
            txt_Dialogue.text = "";
            txt_Name.text = "";

            // 선택된 플래그 설정
            if (!string.IsNullOrEmpty(dialogues[lineCount].flag))
            {
                currentFlag = dialogues[lineCount].flag; // 플래그를 배열이 아닌 문자열로 처리
                Debug.Log($"{currentFlag} 현재 플래그 설정");
                bool foundCurrentLine = false;

                for (int i = 0; i < dialogues.Length; i++)
                {
                    if (i == lineCount)
                    {
                        foundCurrentLine = true;
                        continue;
                    }

                    // 현재 라인 이후에 같은 플래그를 가진 대화를 찾음
                    if (foundCurrentLine && 
                        !string.IsNullOrEmpty(dialogues[i].flag) && 
                        dialogues[i].flag.Equals(currentFlag)) // Contains 대신 Equals 사용
                    {
                        lineCount = i;
                        Debug.Log($"플래그 {currentFlag}를 가진 대화 {i}로 이동");
                        break;
                    }
                }
            }

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

        // 현재 대화에 일러스트 인덱스가 지정되어 있다면 변경
        if (dialogues[lineCount].illustrationIndex >= 0)
        {
            illustrationManager.ChangeIllustration(dialogues[lineCount].illustrationIndex);
        }

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
