using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;

public class PlayEvent : MonoBehaviour
{
    DialogueManager theDM;
    InteractiveEvent interactiveEvent;


    void Start()
    {
        if (GetComponent<InteractiveEvent>() == null)
        {
            Debug.LogError("InteractiveEvent 컴포넌트가 없습니다!");
            return;
        }
        StartCoroutine(WaitForDatabaseAndStart());
    }

    IEnumerator WaitForDatabaseAndStart()
    {
        while (!DatabaseManager.isFinish)
        {
            yield return null;
        }
        
        theDM = FindAnyObjectByType<DialogueManager>();
        interactiveEvent = GetComponent<InteractiveEvent>();
        SetDialogueEvent();
    }

    private void SetDialogueEvent()
    {
        DialogueEvent newEvent = new DialogueEvent();
        newEvent.line = new Vector2(1, DatabaseManager.instance.dialogueDic.Count);
        newEvent.dialogues = DatabaseManager.instance.GetDialogue(1, (int)newEvent.line.y);
        interactiveEvent.SetDialogueEvent(newEvent);
        
        theDM.ShowDialogue(interactiveEvent.GetDialogue());
    }
}