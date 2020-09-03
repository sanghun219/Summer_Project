using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public void FadeIn(float fadeOutTime, System.Action nextEvent = null)
    {
        StartCoroutine(CoFadeIn(fadeOutTime, nextEvent));
    }

    public void FadeOut(float fadeOutTime, System.Action nextEvent = null)
    {
        StartCoroutine(CoFadeOut(fadeOutTime, nextEvent));
    }

    // 투명 -> 불투명
    private IEnumerator CoFadeIn(float fadeOutTime, System.Action nextEvent = null)
    {
        Image sr = this.gameObject.GetComponent<Image>();
        Color tempColor = sr.color;
        while (tempColor.a < 1f)
        {
            tempColor.a += Time.deltaTime / fadeOutTime;
            sr.color = tempColor;

            if (tempColor.a >= 1f) tempColor.a = 1f;

            yield return null;
        }

        sr.color = tempColor;
        if (nextEvent != null) nextEvent();
    }

    // 불투명 -> 투명
    private IEnumerator CoFadeOut(float fadeOutTime, System.Action nextEvent = null)
    {
        Image sr = this.gameObject.GetComponent<Image>();
        Color tempColor = sr.color;
        while (tempColor.a > 0f)
        {
            tempColor.a -= Time.deltaTime / fadeOutTime;
            sr.color = tempColor;

            if (tempColor.a <= 0f) tempColor.a = 0f;

            yield return null;
        }
        sr.color = tempColor;
        if (nextEvent != null) nextEvent();
        this.gameObject.SetActive(false);
    }
}