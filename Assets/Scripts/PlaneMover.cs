using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class PlaneMover : MonoBehaviour
{
    public float speed = 1.0f;
    public float duration = 9.0f;

    private Vector3 originalPosition;

    public VideoPlayer videoPlayer;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
        {
            StartCoroutine(MovePlane());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
    }

    private IEnumerator MovePlane()
    {
        while (true)
        {
            float elapsedTime = 0f;
            Vector3 targetPosition = originalPosition + new Vector3(0, 0, speed * duration);

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            transform.position = originalPosition;

            yield return new WaitForSeconds(0.1f);

            if (!videoPlayer.isPrepared)
            {
                videoPlayer.Prepare();
                while (!videoPlayer.isPrepared)
                {
                    yield return null;
                }
            }

            videoPlayer.Stop();
            videoPlayer.time = 0;
            videoPlayer.Play();
        }
    }
}
