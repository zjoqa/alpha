using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FadeState { FadeIn = 0 , FadeOut , FadeInOut , FadeLoop}

public class FadeEffect : MonoBehaviour
{
    [SerializeField] 
    [Range(0.01f, 10f)]
    private float fadeTime; // fadeSpeed 의 값이 10이면 1초 (값이 클 수록 빠름)
    [SerializeField]
    private AnimationCurve fadeCurve; // 페이드 효과에 사용되는 곡선
    private Image image; // 페이드 효과에 적용되는 검은 바탕 이미지
    private FadeState fadeState; // 페이드 효과 상태

    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnFade(FadeState state)
    {
        // 이전 코루틴이 실행 중이라면 중지
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        fadeState = state;

        switch(fadeState)
        {
            case FadeState.FadeIn:
                currentFadeCoroutine = StartCoroutine(Fade(1,0));
                break;
            case FadeState.FadeOut:
                currentFadeCoroutine = StartCoroutine(Fade(0,1));
                break; 
            case FadeState.FadeInOut:
            case FadeState.FadeLoop:
                currentFadeCoroutine = StartCoroutine(FadeInOut());
                break;
        }
    }

    private IEnumerator FadeInOut()
    {
        while(true)
        {
            // 코루틴 내부에서 코루틴 함수를 호출하면 해당 코루틴 함수가 종료되어야 다음 문장 실행
            yield return StartCoroutine(Fade(1,0)); // Fade In
            yield return StartCoroutine(Fade(0,1)); // Fade Out

            // 1회만 재생하는 상태일 때 break;
            if(fadeState == FadeState.FadeInOut)
                break;
        }
    }

    private IEnumerator Fade(float start, float end)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while( percent < 1)
        {
            //fadeTime 으로 나누어서 fadeTime 시간동안
            // percent 값이 0에서 1로 증가하도록 함
            currentTime += Time.deltaTime;
            percent = currentTime / fadeTime;

            // 알파값을 start부터 end까지 fadeTime 시간 동안 변화시킨다
            Color color = image.color;
            
            color.a = Mathf.Lerp(start, end, fadeCurve.Evaluate(percent));

            image.color = color;

            yield return null;
        }

    }
}
