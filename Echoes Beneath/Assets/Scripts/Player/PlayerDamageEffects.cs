using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageEffects : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Image damageFlashImage;
    [SerializeField] private float flashAlpha = 0.7f;
    [SerializeField] private float flashFadeDuration = 0.3f;

    private Coroutine _flashCoroutine;

    public void PlayDamageFlash()
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        _flashCoroutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        Color flashColor = damageFlashImage.color;
        flashColor.a = flashAlpha;
        damageFlashImage.color = flashColor;

        float t = 0f;
        while (t < flashFadeDuration)
        {
            t += Time.deltaTime;
            float normalized = t / flashFadeDuration;

            flashColor.a = Mathf.Lerp(flashAlpha, 0f, normalized);
            damageFlashImage.color = flashColor;

            yield return null;
        }

        flashColor.a = 0f;
        damageFlashImage.color = flashColor;
        _flashCoroutine = null;
    }
}