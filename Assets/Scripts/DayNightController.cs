using System;
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

    public Animator neonAnimator;
    
    private bool isNight = false;

    private void Start()
    {
        neonAnimator.enabled = false;
    }

    [Button]
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
        if (neonAnimator != null)
        {
            neonAnimator.enabled = isNight;
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
    }

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
