using UnityEngine;
using System.Collections;

public class SkyboxExposureAnimator : MonoBehaviour
{
    public float exposureStart = 0.6f; // Initial exposure value
    public float exposureEnd = 0.2f;   // Target exposure value
    public float fadeDuration = 5.0f;  // Duration of the fade in seconds
    public float rotateSpeed = 0.2f;

    void Start()
    {
        // Start the exposure animation when the script begins
        StartCoroutine(AnimateExposure(exposureStart, exposureEnd, fadeDuration));
    }

    IEnumerator AnimateExposure(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentExposure = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            RenderSettings.skybox.SetFloat("_Exposure", currentExposure);
            yield return null; // Wait for the next frame
        }

        // Ensure the final exposure value is set precisely
        RenderSettings.skybox.SetFloat("_Exposure", endValue);
    }

        void Update()
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
        }
}