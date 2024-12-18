using UnityEngine;

/// <summary>
/// 이 스크립트는 DatabaseManager의 딕셔너리에서 대화 데이터를 가져와서 Unity의 컴포넌트 시스템과 연결하는 브릿지 역할을 수행
/// </summary>
public class InteractiveEvent : MonoBehaviour
{
    [SerializeField]
    private DialogueEvent dialogueEvent = new DialogueEvent();

    private void Awake()
    {
        if (dialogueEvent == null)
            dialogueEvent = new DialogueEvent();
    }

    public Dialogue[] GetDialogue()
    {
        dialogueEvent.dialogues = DatabaseManager.instance.GetDialogue((int)dialogueEvent.line.x, (int)dialogueEvent.line.y);
        return dialogueEvent.dialogues;
    }

    public void SetDialogueEvent(DialogueEvent newEvent)
    {
        dialogueEvent.line = newEvent.line;
        dialogueEvent.dialogues = newEvent.dialogues;
    }
}
