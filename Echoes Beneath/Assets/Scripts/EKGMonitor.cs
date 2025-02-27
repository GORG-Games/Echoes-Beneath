using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class EKGMonitor : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform ekgLineContainer; // Контейнер для линий ЭКГ
    [SerializeField] private Image ekgBackground;
    [SerializeField] private Image ekgLine;
    [SerializeField] private Image ekgHeart;

    [Header("Pulse Prefabs")]
    [SerializeField] private GameObject pulseGreenPrefab;
    [SerializeField] private GameObject pulseYellowPrefab;
    [SerializeField] private GameObject pulseRedPrefab;

    private GameObject currentPulsePrefab;

    [Header("Pulse Settings")]
    [SerializeField] private PulseController pulseController;
    [SerializeField] private float pulseSpeed = 300f;        // Скорость движения импульсов
    [SerializeField] private float timeout = 10f;
    private float timer = 0f;

    private float deleteThreshold;

    [Header("Green Sprites")]
    [SerializeField] private Sprite greenBackground;
    [SerializeField] private Sprite greenLine;
    [SerializeField] private Sprite greenHeart;

    [Header("Yellow Sprites")]
    [SerializeField] private Sprite yellowBackground;
    [SerializeField] private Sprite yellowLine;
    [SerializeField] private Sprite yellowHeart;

    [Header("Red Sprites")]
    [SerializeField] private Sprite redBackground;
    [SerializeField] private Sprite redLine;
    [SerializeField] private Sprite redHeart;
    IEnumerator GeneratePulse()
    {
        // Создаём новый импульс
        GameObject pulse = Instantiate(currentPulsePrefab, ekgLineContainer);
        RectTransform pulseRect = pulse.GetComponent<RectTransform>();

        // Устанавливаем позицию импульса в правой части контейнера
        pulseRect.anchoredPosition = new Vector2(ekgLineContainer.rect.width / 2, 0);

        // Анимация движения импульса
        StartCoroutine(MovePulse(pulseRect));

        // Минимальная задержка между импульсами
        float delay = Mathf.Max(60f / pulseController.CurrentPulse, 0.05f);
        yield return new WaitForSeconds(delay);
    }

    IEnumerator MovePulse(RectTransform pulseRect)
    {
        timer = 0f;
        deleteThreshold = -ekgLineContainer.rect.width / 2 + pulseRect.rect.width;
        while (pulseRect.anchoredPosition.x > deleteThreshold && timer < timeout)
        {
            //pulseRect.localPosition += Vector3.left * pulseSpeed * Time.deltaTime;
            pulseRect.anchoredPosition += Vector2.left * pulseSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(pulseRect.gameObject);
    }
    public void StartPulseGeneration()
    {
        StartCoroutine(GeneratePulse());
    }


    public void UpdateEKGState(int currentHP, int maxHP)
    {
        float hpPercent = (float)currentHP / maxHP;
        if (hpPercent > 0.6f)
        {
            // Green set
            ekgBackground.sprite = greenBackground;
            ekgLine.sprite = greenLine;
            ekgHeart.sprite = greenHeart;
            currentPulsePrefab = pulseGreenPrefab;
        }
        else if (hpPercent > 0.3f)
        {
            // Yellow set
            ekgBackground.sprite = yellowBackground;
            ekgLine.sprite = yellowLine;
            ekgHeart.sprite = yellowHeart;
            currentPulsePrefab = pulseYellowPrefab;
        }
        else
        {
            // Red set
            ekgBackground.sprite = redBackground;
            ekgLine.sprite = redLine;
            ekgHeart.sprite = redHeart;
            currentPulsePrefab = pulseRedPrefab;
        }
    }
}