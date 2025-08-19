using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeIn(0, 1, duration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeIn(1, 0, duration));
    }

    private IEnumerator FadeIn(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = to;
    }
}
