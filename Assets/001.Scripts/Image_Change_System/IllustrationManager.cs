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

    private void Start()
    {
        // Resources/Illustration 폴더에 있는 모든 Sprite를 로드하고 이름순으로 정렬
        illustrations = Resources.LoadAll<Sprite>("Illustration").OrderBy(sprite => sprite.name).ToArray();
        theFade = FindObjectOfType<FadeEffect>();
        ChangeIllustration(0);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ChangeIllustration(NextIllustrationIndex());
        }
    }
    public void ChangeIllustration(int index)
    {
        if(index >= 0 && index < illustrations.Length)
        {
            theFade.OnFade(FadeState.FadeOut);
            illustrationImage.sprite = illustrations[index];
            currentIndex = index;
            theFade.OnFade(FadeState.FadeIn);
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
