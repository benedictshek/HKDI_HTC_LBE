using UnityEngine;

public class PlayWelcomeAudio : MonoBehaviour
{
    [SerializeField] private float waitTimeForDelay = 15f;
    [SerializeField] private float waitTimeForDelay2 = 10f;
    [SerializeField] private float waitTimeForDelay3 = 2f;
    [SerializeField] private float waitTimeForDelay4 = 2f;
    private AudioSource audioSource;
    public AudioClip audioClip;
    public AudioClip audioClip2;
    public AudioClip audioClip3;
    public AudioClip audioClip4;

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
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource component is missing!");
        }
        
        yield return new WaitForSeconds(audioClip.length);
        yield return new WaitForSeconds(waitTimeForDelay2);

        audioSource.clip = audioClip2;
        audioSource.Play();
        
        yield return new WaitForSeconds(audioClip2.length);
        yield return new WaitForSeconds(waitTimeForDelay3);
        
        audioSource.clip = audioClip3;
        audioSource.Play();
        
        yield return new WaitForSeconds(audioClip3.length);
        yield return new WaitForSeconds(waitTimeForDelay4);
        
        audioSource.clip = audioClip4;
        audioSource.Play();
    }
}
