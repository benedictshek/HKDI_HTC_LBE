using System.Collections;
using UnityEngine;
using VInspector;

public class DayNightController : MonoBehaviour
{
    [Header("Sky Boxes")]
    public Material daySkybox;
    public Material nightSkybox;
    
    [Header("Lighting")]
    public Light directionalLight;
    public float dayDirectionalIntensity = 1.6f;
    public float nightDirectionalIntensity = 0f;
    
    [Header("Ambient Lighting")]
    public float dayAmbientIntensity = 1.85f;
    public float nightAmbientIntensity = 0.1f;
    
    [Header("Fog Settings")]
    public float fogDensityDay = 0.015f;
    public float fogDensityNight = 0f;
    
    [Header("Point Lights")]
    public Light[] pointLights;
    
    [Header("Emission Materials")]
    public Material[] emissionMats;

    public Animator[] neonAnimators;
    
    private bool isNight = false;
    
    [Header("Transition Settings")]
    public float transitionDuration = 10f;

    private bool isTransitioning = false;

    private void Start()
    {
        foreach (var animator in neonAnimators)
        {
            animator.enabled = false;
        }
    }

    [Button]
    public void ToggleDayNight()
    {
        if (!isTransitioning)
            StartCoroutine(TransitionDayNight(!isNight));
    }
    
    private IEnumerator TransitionDayNight(bool toNight)
    {
        isTransitioning = true;

        float elapsed = 0f;

        // Cache start and target values
        float startDirIntensity = directionalLight.intensity;
        float endDirIntensity = toNight ? nightDirectionalIntensity : dayDirectionalIntensity;

        float startAmbient = RenderSettings.ambientIntensity;
        float endAmbient = toNight ? nightAmbientIntensity : dayAmbientIntensity;

        float startFog = RenderSettings.fogDensity;
        float endFog = toNight ? fogDensityNight : fogDensityDay;

        // Set skybox immediately (or replace with blended shader logic)
        RenderSettings.skybox = toNight ? nightSkybox : daySkybox;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;

            directionalLight.intensity = Mathf.Lerp(startDirIntensity, endDirIntensity, t);
            RenderSettings.ambientIntensity = Mathf.Lerp(startAmbient, endAmbient, t);
            RenderSettings.fogDensity = Mathf.Lerp(startFog, endFog, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Finalize values after transition
        directionalLight.intensity = endDirIntensity;
        RenderSettings.ambientIntensity = endAmbient;
        RenderSettings.fogDensity = endFog;

        // Apply night-only effects AFTER transition
        bool enableNightEffects = toNight;

        foreach (var animator in neonAnimators)
            animator.enabled = enableNightEffects;

        foreach (var light in pointLights)
            light.enabled = enableNightEffects;

        foreach (var mat in emissionMats)
        {
            if (mat == null) continue;

            if (enableNightEffects)
            {
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }

        isNight = toNight;
        isTransitioning = false;
    }

    /*[Button]
    public void ToggleDayNight()
    {
        isNight = !isNight;
        
        // Skybox
        RenderSettings.skybox = isNight ? nightSkybox : daySkybox;
        // Ambient lighting intensity (environment lighting)
        RenderSettings.ambientIntensity = isNight ? nightAmbientIntensity : dayAmbientIntensity;
        // Set fog density
        RenderSettings.fogDensity = isNight ? fogDensityNight : fogDensityDay;
        
        if (directionalLight != null)
        {
            directionalLight.intensity = isNight ? nightDirectionalIntensity : dayDirectionalIntensity;
        }
        
        // Animator control
        foreach (var animator in neonAnimators)
        {
            if (animator != null)
            {
                animator.enabled = isNight;
            }
        }
        
        // Point lights
        foreach (var point in pointLights)
        {
            if (point != null)
                point.enabled = isNight;
        }
        
        // Emission control
        foreach (var mat in emissionMats)
        {
            if (mat != null)
            {
                if (isNight)
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                }
                else
                {
                    mat.DisableKeyword("_EMISSION");
                    mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                }
            }
        }
    }*/

    private void OnDisable()
    {
        foreach (var mat in emissionMats)
        {
            if (mat != null)
            {
                mat.DisableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }
    }
}
