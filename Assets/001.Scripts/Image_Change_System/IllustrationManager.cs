using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class IllustrationManager : MonoBehaviour
{
    public Image illustrationImage;  // Canvas에 배치한 UI Image
    public Sprite[] illustrations;  // 미리 로드된 일러스트 Sprite 배열
    public FadeEffect theFade;

    private int currentIndex = 0;
    private string currentName;

    private void Start()
    {
        // Resources/Illustration 폴더에 있는 모든 Sprite를 로드하고 이름순으로 정렬
        illustrations = Resources.LoadAll<Sprite>("Illustration").OrderBy(sprite => sprite.name).ToArray();
        theFade = FindObjectOfType<FadeEffect>();
        ChangeIllustration(0);
    }

    public void ChangeIllustration(int index, string name = null)
    {
        if(index < illustrations.Length)
        {
            // 일러스트 인덱스가 비어있지 않을 때만 변경 작업 수행
            if(!string.IsNullOrEmpty(index.ToString()))
            {
                // 이름이 있고, 이전 대사와 현재 대사의 이름이 다를 때만 페이드
                if(name != null && currentName != name)
                {
                    theFade.OnFade(FadeState.FadeOut);
                    illustrationImage.sprite = illustrations[index];
                    theFade.OnFade(FadeState.FadeIn);
                    currentName = name;
                }
                else
                {
                    // 같은 캐릭터의 연속된 대사일 경우 페이드 없이 일러스트만 변경
                    illustrationImage.sprite = illustrations[index];
                }
                currentIndex = index;
            }
            // 일러스트 인덱스가 비어있으면 아무 작업도 하지 않음 (이전 일러스트 유지)
        }
        else
        {
            Debug.LogWarning("일러스트 인덱스가 범위를 벗어났습니다.");
        }
    }

    public int NextIllustrationIndex()
    {
        return (currentIndex + 1) % illustrations.Length;
    }
}
