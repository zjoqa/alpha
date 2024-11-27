using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // 대화 목록을 저장할 임시 리스트
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); // csv 파일 로드하여 csvData에 저장

        string[] data = csvData.text.Split(new char[] {'\n'}); // csv 파일을 줄 단위로 분리하여 data에 저장


        for (int i = 1; i < data.Length;) // 첫 번째 줄은 헤더이므로 1부터 시작
        {
            string[] row = data[i].Split(new char[] { ',' }); // 나눈 줄을 쉼표(,)로 분리하여 row에 저장

            // CSV 파일의 각 행에 대한 새로운 Dialogue 객체 생성
            // 이 객체는 대화자 이름, 대화 내용, 선택지 등의 정보를 저장
            Dialogue dialogue = new Dialogue();
            dialogue.name = row[1]; // 대화자 이름
            List<string> contextList = new List<string>(); // 대화 내용을 저장할 임시 리스트

            do
            {
                contextList.Add(row[2]); // 임시 리스트에 대화 내용 추가
                
                // 선택지가 존재하는 경우
                if (row.Length >= 6 && !string.IsNullOrEmpty(row[3]) && !string.IsNullOrEmpty(row[4]))
                {
                    string[] choiceTexts = row[3].Split('|'); // 선택지 텍스트를 쉼표(,)로 분리하여 choiceTexts에 저장
                    string[] nextIndices = row[4].Split('|'); // 다음 대화 인덱스를 쉼표(,)로 분리하여 nextIndices에 저장
                    
                    if (choiceTexts.Length == nextIndices.Length) // 선택지 텍스트와 다음 대화 인덱스의 개수가 같은 경우
                    {
                        dialogue.choices = new DialogueChoice[choiceTexts.Length]; // 선택지 개수만큼 DialogueChoice을 담을 수 있는 빈 배열만 생성
                        for (int j = 0; j < choiceTexts.Length; j++) // 선택지 개수만큼 반복
                        {
                            dialogue.choices[j] = new DialogueChoice(); // 선택지 개수만큼 반복하여 실제 DialogueChoice 객체 할당
                            dialogue.choices[j].choiceText = choiceTexts[j].Trim(); // 선택지 텍스트 설정
                            dialogue.choices[j].nextDialogueIndex = int.Parse(nextIndices[j].Trim()); // 다음 대화 인덱스 설정
                        }
                    }
                }
                else if(row.Length >= 6 && string.IsNullOrEmpty(row[3])) // 선택 대사가 없는 경우
                {
                    string nextDialogueIndex = row[4].Trim();
                    if(!string.IsNullOrEmpty(nextDialogueIndex))
                    {
                        // TryParse를 사용하여 안전하게 변환
                        if(int.TryParse(nextDialogueIndex, out int result))
                        {
                            dialogue._nextDialogueIndex = result;
                        }
                        else
                        {
                            Debug.LogWarning($"다음 대화 인덱스 값 '{nextDialogueIndex}'을(를) 숫자로 변환할 수 없습니다.");
                        }
                    }
                    else if(!string.IsNullOrEmpty(row[5])) // 대사 종료 여부가 존재하면
                    {
                        dialogue.isEnd = true; // 대사 종료 여부 설정
                    }
                }
                // 선택지가 없는 경우
                if(++i < data.Length) // 다음 줄로 이동 
                {
                    row = data[i].Split(new char[] { ',' }); // 다음 줄을 쉼표(,)로 분리하여 row에 저장
                }
                else // 다음 줄이 없는 경우
                {
                    break; // 반복문 종료
                }
            } while (row[0].ToString() == ""); // 현재 줄의 첫번째 요소가 비어있으면 반복

            dialogue.contexts = contextList.ToArray(); // 임시 리스트를 배열로 변환하여 dialogue.contexts 에 저장
            // dialogue 는 for문이 새로 반복할 때 마다 새롭게 객체가 추가되므로 임시 List인 dialogueList에 저장
            dialogueList.Add(dialogue); 
        }



        return dialogueList.ToArray(); // 임시 List인 dialogueList를 배열로 변환하여 반환
    }
}