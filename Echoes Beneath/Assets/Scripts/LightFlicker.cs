using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [System.Serializable]
    public class FlickerLight
    {
        public Light2D light;
        [HideInInspector] public float originalIntensity;
    }

    [SerializeField] private FlickerLight[] lightsToFlicker;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float lightOnDuration = 1.0f;
    [SerializeField] private float lightOffDuration = 0.3f;

    private Coroutine flickerCoroutine;

    private void Awake()
    {
        foreach (var flickerLight in lightsToFlicker)
        {
            if (flickerLight.light != null)
                flickerLight.originalIntensity = flickerLight.light.intensity;
        }
    }

    public void StartFlicker()
    {
        if (flickerCoroutine == null)
            flickerCoroutine = StartCoroutine(FlickerLoop());
    }

    public void StopFlicker()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        foreach (var flickerLight in lightsToFlicker)
        {
            if (flickerLight.light != null)
                flickerLight.light.intensity = flickerLight.originalIntensity;
        }
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            // Погасить свет
            yield return StartCoroutine(FadeLights(1f, 0f, fadeDuration));
            yield return new WaitForSeconds(lightOffDuration);

            // Включить свет обратно
            yield return StartCoroutine(FadeLights(0f, 1f, fadeDuration));
            yield return new WaitForSeconds(lightOnDuration);
        }
    }

    private IEnumerator FadeLights(float fromFactor, float toFactor, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            foreach (var flickerLight in lightsToFlicker)
            {
                if (flickerLight.light != null)
                {
                    float target = Mathf.Lerp(flickerLight.originalIntensity * fromFactor, flickerLight.originalIntensity * toFactor, t);
                    flickerLight.light.intensity = target;
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (var flickerLight in lightsToFlicker)
        {
            if (flickerLight.light != null)
                flickerLight.light.intensity = flickerLight.originalIntensity * toFactor;
        }
    }
}