using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenController : MonoBehaviour
{
    [SerializeField] private CanvasGroup deathScreenGroup;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] SceneLoader _sceneLoader;

    private bool isActive = false;

    void Start()
    {
        deathScreenGroup.alpha = 0f;
        deathScreenGroup.interactable = false;
        deathScreenGroup.blocksRaycasts = false;
        audioMixer.SetFloat("MasterVolume", 0f);
    }

    public void ShowDeathScreen()
    {
        if (isActive) return;
        isActive = true;

        StartCoroutine(FadeIn());
        audioMixer.SetFloat("MasterVolume", -80f); // Глушим звук
        // Можно выключить управление игроком тут
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            deathScreenGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        deathScreenGroup.alpha = 1f;
        deathScreenGroup.interactable = true;
        deathScreenGroup.blocksRaycasts = true;
    }
}