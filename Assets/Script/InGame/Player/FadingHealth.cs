using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어가 피해를 입을시 카메라 가장자리 부근에서 붉은색으로 효과를 내주는 클래스
/// </summary>
public class FadingHealth : MonoSingleton<FadingHealth>
{
    public Image fadingHealthImg;
    
    private float fadingAlpha = 0.2f;   //Fade max alpha value
    private bool isLoop = false;
    private bool isPlay = false;

    private Coroutine fadeingCoroutine;

    /// <summary>
    /// 효과를 일정시간 마다 보여줄 때
    /// </summary>
    /// <param name="fadeValue"></param>
    public void FadingStart(float fadeValue)
    {
        if (!isPlay)
        {
            isPlay = true;
            isLoop = true;
            fadingAlpha = fadeValue;
            fadeingCoroutine = StartCoroutine(FadingCo());
        }
    }
    public void FadingStop()
    {
        isLoop = false;
    }
    /// <summary>
    /// 효과를 한번 보여줄 때
    /// </summary>
    /// <param name="fadeValue"></param>
    public void FadingTrigger(float fadeValue)
    {
        if (!isPlay)
        {
            isPlay = true;
            isLoop = false;
            fadingAlpha = fadeValue;
            fadeingCoroutine = StartCoroutine(FadingCo());
        }
    }
    IEnumerator FadingCo()
    {
        Color fadeColor = new Color();
        fadeColor = Color.white;
        fadeColor.a = 0.0f;
        
        do
        {
            while (fadeColor.a <= fadingAlpha)
            {
                fadeColor.a += 0.01f;
                fadingHealthImg.color = fadeColor;

                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(0.3f);

            while (fadeColor.a >= 0.0f)
            {
                fadeColor.a -= 0.01f;
                fadingHealthImg.color = fadeColor;

                yield return new WaitForSeconds(0.01f);
            }
        } while (isLoop);

        isPlay = false;
    }
}