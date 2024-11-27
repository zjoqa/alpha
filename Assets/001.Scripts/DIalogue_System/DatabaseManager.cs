using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance; // 싱글톤 인스턴스

    [SerializeField] string csv_FileName; // CSV 파일명
    
    /// <summary>
    /// 대화 정보를 저장하는 딕셔너리
    /// </summary>
    public Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>(); // 대화 정보를 저장하는 딕셔너리

    public static bool isFinish = false; // 대화 정보 로드 완료 여부

    void Awake()
    {
        if(instance == null) // 싱글톤 인스턴스가 없는 경우
        {
            instance = this; // 자기 자신을 할당
            DialogueParser theParser = GetComponent<DialogueParser>(); 
            
            // DialogueParser 컴포넌트 존재 여부 확인
            if (theParser == null)
            {
                Debug.LogError("DialogueParser 컴포넌트가 없습니다!");
                return;
            }

            // csv 파일명이 비어있는지 확인
            if (string.IsNullOrEmpty(csv_FileName))
            {
                Debug.LogError("CSV 파일명이 설정되지 않았습니다!");
                return;
            }

            Dialogue[] dialogues = theParser.Parse(csv_FileName); // theParser.Parse() 에서 파싱이 끝난 후 List를 배열로 반환하는데 그 정보를 dialogues에 저장  
            for(int i = 0; i < dialogues.Length; i++) // 대화 정보 길이만큼 반복
            {
                dialogueDic.Add(i+1, dialogues[i]); // 대화 정보를 딕셔너리에 추가 ( +1 은 대화 인덱스가 1부터 시작하기 때문)
            }
            isFinish = true; // 대화 정보 로드 완료 
        }
    }
    /// <summary>
    /// dialougeDic 에서 대화 정보 (Dialogue[])를 가져오는 함수
    /// </summary>
    /// <param name="_StartNum">시작 대화 인덱스</param>
    /// <param name="_EndNum">끝 대화 인덱스</param>
    /// <returns>대화 정보 (Dialogue[])</returns>
    public Dialogue[] GetDialogue(int _StartNum, int _EndNum)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // 임시 대화 리스트 생성

        for(int i = 0 ; i <= _EndNum - _StartNum; i++)
        {
            dialogueList.Add(dialogueDic[_StartNum + i]); // 대화 정보를 임시 대화 리스트에 추가
        }
        return dialogueList.ToArray(); // 임시 대화 리스트를 배열로 변환 후 return
    }

    // 데이터베이스 데이터 교체 함수 (챕터 전환시)
    public void ReloadDatabase(string newCsvFileName)
    {
        // 기존 데이터 초기화
        dialogueDic.Clear();
        isFinish = false;
        
        // 새로운 CSV 파일명 설정
        csv_FileName = newCsvFileName;
        
        // DialogueParser로 새로운 데이터 파싱
        DialogueParser theParser = GetComponent<DialogueParser>();
        if (theParser != null && !string.IsNullOrEmpty(csv_FileName)) // 파싱 컴포넌트가 존재하고, CSV 파일명이 비어있지 않은 경우
        {
            Dialogue[] dialogues = theParser.Parse(csv_FileName); // theParser.Parse() 에서 파싱이 끝난 후 List를 배열로 반환하는데 그 정보를 dialogues에 저장  
            for(int i = 0; i < dialogues.Length; i++) // 대화 정보 길이만큼 반복
            {
                dialogueDic.Add(i+1, dialogues[i]); // 대화 정보를 딕셔너리에 추가
            }
            isFinish = true; // 대화 정보 로드 완료
        }
    }

    //챕터 전환 예시
    /*
    void ChangeChapter()
    {
        // 데이터 리로드
        DatabaseManager.instance.ReloadDatabase("chapter1");
        
        // 로딩 완료 대기
        StartCoroutine(WaitForDatabaseLoad());
    }

    IEnumerator WaitForDatabaseLoad()
    {
        while (!DatabaseManager.isFinish)
        {
            yield return null;
        }
        // 로딩 완료 후 처리    
        Debug.Log("새로운 챕터 데이터 로드 완료!");
    }
    */

    public int GetDialogueLength()
    {
        return dialogueDic.Count;
    }
}
