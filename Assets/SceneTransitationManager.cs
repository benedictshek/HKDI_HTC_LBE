using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : NetworkBehaviour
{
    [Header("Transition Settings")]
    public Animator animator;
    public string nextSceneName;
    public float animationDuration = 15f; // Wait time before fade
    public float fadeDuration = 4f;

    private bool transitionStarted = false;
    
    public ScreenFader screenFader;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip welcomeClip;
    public AudioClip guideClip;
    public float welcomeDelay = 10f;
    public float guideDelay = 30f; // Time after which player is guided
    
    private bool canTriggerTransition;
    private bool isServerReady;

    public GameObject Teleport;

    private void Start()
    {
        Teleport.SetActive(false);
    }

    private void Update()
    {
        if (!IsServer || isServerReady) return;
        isServerReady = true;

        StartCoroutine(AudioGuideSequence());
    }
    
    private IEnumerator AudioGuideSequence()
    {
        yield return new WaitForSeconds(welcomeDelay);
        
        // Play welcome audio
        PlayAudioClientRpc(welcomeClip.name);

        // Wait for the guide delay
        yield return new WaitForSeconds(guideDelay);

        // Play guide audio
        PlayAudioClientRpc(guideClip.name);

        // Enable trigger after guide audio
        EnableTriggerClientRpc();
        Teleport.SetActive(true);
    }
    
    [ClientRpc]
    private void PlayAudioClientRpc(string clipName)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = null;

        if (clipName == welcomeClip.name)
            clipToPlay = welcomeClip;
        else if (clipName == guideClip.name)
            clipToPlay = guideClip;

        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }
    
    [ClientRpc]
    private void EnableTriggerClientRpc()
    {
        canTriggerTransition = true;
        Teleport.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTriggerTransition) return;
        
        if (other.GetComponentInParent<NetworkObject>().IsOwner)
        {
            RequestSceneTransitionServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSceneTransitionServerRpc(ServerRpcParams rpcParams = default)
    {
        if (transitionStarted) return;
        transitionStarted = true;

        PlayAnimationClientRpc();

        // Delay for animation and fade, then load scene
        StartCoroutine(DelayedSceneChange());
    }

    [ClientRpc]
    private void PlayAnimationClientRpc()
    {
        if (animator != null)
        {
            animator.SetTrigger("PlayAnimation");
        }
    }

    private IEnumerator DelayedSceneChange()
    {
        yield return new WaitForSeconds(animationDuration - fadeDuration);
        
        FadeInClientRpc();
        
        yield return new WaitForSeconds(fadeDuration);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // Use Netcode SceneManager to sync scene load
            NetworkManager.SceneManager.LoadScene(
                nextSceneName,
                LoadSceneMode.Single
            );
        }
        else
        {
            Debug.LogError("Next scene name is not set.");
        }
    }
    
    [ClientRpc]
    private void FadeInClientRpc()
    {
        screenFader.FadeIn(fadeDuration);
    }
}
