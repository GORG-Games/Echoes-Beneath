using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextTrigger : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI _displayText; // Ссылка на текстовый компонент
    [SerializeField] private float _fadeDuration = 0.5f;   // Длительность анимации появления/исчезновения

    private Color _originalColor;  // Исходный цвет текста
    [SerializeField] private LayerMask _playerLayer;
    private bool _hasTriggered = false;
    private void Start()
    {
        // Сохраняем исходный цвет и делаем текст прозрачным
        _displayText.gameObject.SetActive(true);
        _originalColor = _displayText.color;
        _displayText.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject) && !_hasTriggered) // Проверка, что вошёл игрок
        {
            ShowText();
            _hasTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject)) // Проверка, что вышел игрок
        {
            HideText();
        }
    }

    private void ShowText()
    {
        _displayText.DOFade(1f, _fadeDuration); // Плавное появление текста
    }

    private void HideText()
    {
        _displayText.DOFade(0f, _fadeDuration); // Плавное исчезновение текста
    }
}