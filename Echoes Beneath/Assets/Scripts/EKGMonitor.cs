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
    [SerializeField] private float timeout = 10f;
    private float timer = 0f;

    private float deleteThreshold;
    IEnumerator GeneratePulse()
    {
        // ������ ����� �������
        GameObject pulse = Instantiate(pulsePrefab, ekgLineContainer);
        RectTransform pulseRect = pulse.GetComponent<RectTransform>();

        // ������������� ������� �������� � ������ ����� ����������
        pulseRect.anchoredPosition = new Vector2(ekgLineContainer.rect.width / 2, 0);

        // �������� �������� ��������
        StartCoroutine(MovePulse(pulseRect));

        // ����������� �������� ����� ����������
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
}