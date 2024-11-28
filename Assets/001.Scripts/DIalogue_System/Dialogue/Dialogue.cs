using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextDialogueIndex;
    public int illustrationIndex; // 선택지에 따른 일러스트 인덱스 추가
}

[System.Serializable]
public class Dialogue
{
    [Tooltip("대사 치는 캐릭터 이름")]
    public string name;

    [Tooltip("대사 내용")]
    public string[] contexts;
    
    [Tooltip("선택지")]
    public DialogueChoice[] choices;

    [Tooltip("대화 종료 여부")]
    public bool isEnd;

    public int _nextDialogueIndex;
    
    [Tooltip("일러스트 인덱스")]
    public int illustrationIndex = -1; // -1은 일러스트 변경 없음을 의미
}

[System.Serializable]
public class DialogueEvent
{
    public string name;

    public Vector2 line;
    public Dialogue[] dialogues;
}
