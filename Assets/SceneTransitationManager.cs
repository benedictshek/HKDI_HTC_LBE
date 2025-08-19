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

    private void OnTriggerEnter(Collider other)
    {
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
