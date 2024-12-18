using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EKGMonitor : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform ekgLineContainer; // ��������� ��� ����� ���
    [SerializeField] private GameObject pulsePrefab;         // ������ ��� �������� ���

    [Header("Pulse Settings")]
    [SerializeField] private PulseController pulseController;
    [SerializeField] private float pulseSpeed = 300f;        // �������� �������� ���������


    void Start()
    {
        StartCoroutine(GeneratePulse());
    }

    IEnumerator GeneratePulse()
    {
        while (true)
        {
            // ������ ����� �������
            GameObject pulse = Instantiate(pulsePrefab, ekgLineContainer);
            RectTransform pulseRect = pulse.GetComponent<RectTransform>();

            // �������� �������� ��������
            StartCoroutine(MovePulse(pulseRect));

            // �������� ����� ���������� ������� �� �������� ������
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