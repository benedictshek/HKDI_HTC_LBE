using UnityEngine;
    using System.Collections; // Required for Coroutines

public class EmissionControl : MonoBehaviour
    {
        public Material targetMaterial; // Assign your material in the Inspector
        public Color emissionColor = Color.white; // Desired emission color
        public float emissionIntensity = 1.0f; // Desired emission intensity

        void Start()
        {
            // Start the coroutine to activate emission after 5 seconds
            StartCoroutine(ActivateEmissionAfterDelay(5f));
        }

        IEnumerator ActivateEmissionAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (targetMaterial != null)
            {
                // Enable the _EMISSION keyword
                targetMaterial.EnableKeyword("_EMISSION");

                // Set the emission color and intensity
                // Note: For HDRP, you might need to use _EmissiveColor and adjust the intensity directly
                targetMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);

                // Optional: Update global illumination if needed (for baked/realtime GI)
                // DynamicGI.UpdateEnvironment(); 
            }
            else
            {
                Debug.LogWarning("Target Material is not assigned in EmissionController script!");
            }
        }
    }