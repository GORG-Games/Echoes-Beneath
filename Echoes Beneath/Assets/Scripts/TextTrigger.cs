using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextTrigger : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI _displayText; // ������ �� ��������� ���������
    [SerializeField] private float _fadeDuration = 0.5f;   // ������������ �������� ���������/������������

    private Color _originalColor;  // �������� ���� ������
    [SerializeField] private LayerMask _playerLayer;
    private bool _hasTriggered = false;
    private void Start()
    {
        // ��������� �������� ���� � ������ ����� ����������
        _displayText.gameObject.SetActive(true);
        _originalColor = _displayText.color;
        _displayText.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject) && !_hasTriggered) // ��������, ��� ����� �����
        {
            ShowText();
            _hasTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject)) // ��������, ��� ����� �����
        {
            HideText();
        }
    }

    private void ShowText()
    {
        _displayText.DOFade(1f, _fadeDuration); // ������� ��������� ������
    }

    private void HideText()
    {
        _displayText.DOFade(0f, _fadeDuration); // ������� ������������ ������
    }
}