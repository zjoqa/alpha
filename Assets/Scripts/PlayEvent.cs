using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayEvent : MonoBehaviour
{

    DialogueManager theDM;
    InteractiveEvent interactiveEvent;
    void Start()
    {
        theDM = FindAnyObjectByType<DialogueManager>(); // 대화창 매니저 찾아오기
        interactiveEvent = GetComponent<InteractiveEvent>();
        theDM.ShowDialogue(interactiveEvent.GetDialogue());
    }  
}
