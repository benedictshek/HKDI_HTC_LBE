using UnityEngine;

public class PlayWelcomeAudio : MonoBehaviour
{
    [SerializeField] private float waitTimeForDelay = 15f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(PlayAudioWithDelay(waitTimeForDelay));
    }

    private System.Collections.IEnumerator PlayAudioWithDelay(float delay)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Play the audio clip
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource component is missing!");
        }
    }
}
