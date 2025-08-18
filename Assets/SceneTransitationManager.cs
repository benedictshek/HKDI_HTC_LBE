using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitationManager : MonoBehaviour
{
public Animator animator;
public Image blackScreen; // Assign the black screen Image in the Inspector
public float fadeDuration = 1f;
public string nextSceneName; // Name of the scene to load

private void Start()
{
blackScreen.gameObject.SetActive(true);
blackScreen.color = new Color(1, 1, 1, 0); // Start transparent
}

private void OnTriggerEnter(Collider other)
{
Debug.Log("Trigger entered by: " + other.gameObject.name);
if (other.CompareTag("MainCamera")) // Ensure the collider is triggered by the player
{
animator.SetTrigger("PlayAnimation"); 
StartCoroutine(FadeToBlackAndLoadScene()); // Replace with your next scene name
}
}

private IEnumerator FadeToBlackAndLoadScene()
{
// Fade to black
for (float t = 0; t < fadeDuration; t += Time.deltaTime)
{
float normalizedTime = t / fadeDuration;
blackScreen.color = new Color(1, 1, 1, normalizedTime);
yield return null;
}

blackScreen.color = new Color(0, 0, 0, 1); // Ensure it's fully opaque

// Load the next scene
SceneManager.LoadScene(nextSceneName);
}
}