using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    사용자 정의 클래스는 인스펙터에서 다룰 수 없습니다. 
    [System.Serializable]을 붙이면 사용자 정의 클래스나 구조체를 Unity의 직렬화 시스템에서 다룰 수 있게 됩니다.
*/

[System.Serializable] // 직렬화 , 시스템에서 다룰 수 있게 함
public class DialogueChoice 
{
    public string choiceText; // 선택지 텍스트
    public int nextDialogueIndex; // 스킵할 대화 인덱스
    public int illustrationIndex; // 선택지에 따른 일러스트 인덱스 추가
    public string[] flag; // 선택지에 따른 여러 플래그 추가
}

[System.Serializable]
public class Dialogue
{
    [Tooltip("대사 치는 캐릭터 이름")]
    public string name;

    [Tooltip("대사 내용")]
    public string[] contexts;
    
    [Tooltip("선택지")]
    public DialogueChoice[] choices; // 선택지 클래스 배열

    [Tooltip("대화 종료 여부")]
    public bool isEnd;

    public int _nextDialogueIndex; // 다음 대화 인덱스
    
    [Tooltip("일러스트 인덱스")]
    public int illustrationIndex = -1; // -1은 일러스트 변경 없음을 의미
    
    [Tooltip("플래그")]
    public string flag; // 대화에 필요한 플래그
}

[System.Serializable]
public class DialogueEvent 
{
    public string name; //

    public Vector2 line; // 대화 시작 위치
    public Dialogue[] dialogues;
}
