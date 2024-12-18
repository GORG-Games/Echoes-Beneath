using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EKGMonitor : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform ekgLineContainer; // Контейнер для линий ЭКГ
    [SerializeField] private GameObject pulsePrefab;         // Префаб для импульса ЭКГ

    [Header("Pulse Settings")]
    [SerializeField] private PulseController pulseController;
    [SerializeField] private float pulseSpeed = 300f;        // Скорость движения импульсов


    void Start()
    {
        StartCoroutine(GeneratePulse());
    }

    IEnumerator GeneratePulse()
    {
        while (true)
        {
            // Создаём новый импульс
            GameObject pulse = Instantiate(pulsePrefab, ekgLineContainer);
            RectTransform pulseRect = pulse.GetComponent<RectTransform>();

            // Анимация движения импульса
            StartCoroutine(MovePulse(pulseRect));

            // Задержка между импульсами зависит от текущего пульса
            float delay = 60f / pulseController.currentPulse;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator MovePulse(RectTransform pulseRect)
    {
        while (pulseRect.anchoredPosition.x > -ekgLineContainer.rect.width)
        {
            pulseRect.anchoredPosition += Vector2.left * pulseSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(pulseRect.gameObject);
    }
    public void StartPulseGeneration()
    {
        StartCoroutine(GeneratePulse());
    }
}