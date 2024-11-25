using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceButton : MonoBehaviour
{
    private DialogueChoice choiceData;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text choiceText;

    private void Awake()
    {
        button = GetComponent<Button>();
        choiceText = GetComponentInChildren<Text>();
    }

    public void Setup(DialogueChoice choice, System.Action<DialogueChoice> onChoiceSelected)
    {
        choiceData = choice; // 넘어온 인자로 choiceData 초기화
        choiceText.text = choice.choiceText; // 넘어온 인자로 choiceText 초기화
        button.onClick.AddListener(() => onChoiceSelected(choiceData)); // 버튼 클릭 시 onChoiceSelected 함수 호출
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners(); // 버튼 클릭 시 이벤트 제거
    }
} 