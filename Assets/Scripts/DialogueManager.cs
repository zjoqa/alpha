using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    Dialogue[] dialogues;

    bool isDialogue = false; // 현재 대화중인지
    bool isNext = false; // 다음 대사 대기

    int lineCount = 0; // 대화 카운트
    int contextCount = 0; // 대사 카운트

    [Header("텍스트 출력 딜레이")]
    [SerializeField] float textDelay;

    private void Update() {
        if(isDialogue) // 대화중인 경우
        {
            if(isNext&&Input.GetKeyDown(KeyCode.Space)) // 대화 중 스페이스바 입력 시
            {
                isNext = false; 
                txt_Dialogue.text = ""; // 대사 초기화
                if(++contextCount<dialogues[lineCount].contexts.Length) // contextcount 증가한 값이 대사 개수보다 작은 경우
                {
                    StartCoroutine(TypeWriter()); // 타자 효과 시작
                }
                else // contextCount 가 대사 개수를 초과한 경우
                {
                    contextCount = 0; // contextCount 초기화
                    if(++lineCount < dialogues.Length) // lineCount 증가한 값이 대화 개수보다 작은 경우
                    {
                        StartCoroutine(TypeWriter()); // 타자 효과 시작
                    }
                    else
                    {
                        EndDialogue();
                    }
                }   
            }
        }
    }


    public void ShowChoices(DialogueChoice[] choices)
    {
        if(choices == null || choices.Length == 0) return;

        choicePanel.SetActive(true);

        float buttonSpacing = 60f;
        float startY = 100f;

        for(int i = 0; i < choices.Length; i++)
        {
            Button choiceButton = Instantiate(choicePrefab, choicePanel.transform);
            RectTransform rectTransform = choiceButton.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, startY - (buttonSpacing * i));
            rectTransform.sizeDelta = new Vector2(600f, 100f);

            DialogueChoiceButton choiceButtonComponent = choiceButton.GetComponent<DialogueChoiceButton>();
            choiceButtonComponent.Setup(choices[i], OnChoiceSelected);
        }
    }


    public void OnChoiceSelected(DialogueChoice choice)
    {
        choicePanel.SetActive(false);
        ClearChoices();
        
        if (choice.nextDialogueIndex >= 0 && choice.nextDialogueIndex < dialogues.Length)
        {
            lineCount = choice.nextDialogueIndex - 1;
            contextCount = 0;
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
        foreach(Transform child in choicePanel.transform)
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
    }


    IEnumerator TypeWriter()
    {
        string t_ReplaceText = dialogues[lineCount].contexts[contextCount];
        t_ReplaceText = t_ReplaceText.Replace("'", ",");

        txt_Name.text = dialogues[lineCount].name;
        for(int i = 0; i < t_ReplaceText.Length; i++)
        {
            txt_Dialogue.text += t_ReplaceText[i];
            yield return new WaitForSeconds(textDelay);
        }

        // 선택지가 존재하고 대사가 끝나면 선택지 표시
        if(dialogues[lineCount].choices != null && dialogues[lineCount].choices.Length > 0)
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
