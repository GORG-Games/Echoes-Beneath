using System.Collections;
#if UNITY_EDITOR
    using TMPro.EditorUtilities;
    using UnityEditor.Tilemaps;
#endif
using UnityEngine;

public class Level2_ElevatorSwitch : MonoBehaviour
{
    [Header("Door Animator References")]
    [SerializeField] private Animator leftDoorAnimator;
    [SerializeField] private Animator rightDoorAnimator;

    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt; // Подсказка "Нажмите E"
    [SerializeField] private GameObject errorText;
    [SerializeField] private GameObject errorText2;

    private Coroutine _coroutine;
    [SerializeField] private LayerMask _playerLayer;

    private bool isPlayerInZone = false;
    private bool hasActivated = false;
    public bool isPowered = false;  

    void Update()
    {
        if (isPlayerInZone && isPowered && !hasActivated && Input.GetKeyDown(KeyCode.E))
        {
            ActivateElevator();
        }
        else if (isPlayerInZone && !isPowered && Input.GetKeyDown(KeyCode.E))
        {
            _coroutine = StartCoroutine(ErrorTextDisplay());
        }
    }

    void ActivateElevator()
    {
        leftDoorAnimator.SetBool("isOpen", true);
        rightDoorAnimator.SetBool("isOpen", true);

        hasActivated = true;
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    IEnumerator ErrorTextDisplay()
    {
        errorText.SetActive(true); 

        yield return new WaitForSeconds(2f);

        errorText.SetActive(false);
        
        errorText2.SetActive(true); 

        yield return new WaitForSeconds(2f);

        errorText2.SetActive(false);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject))
        {
            isPlayerInZone = true;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject))
        {
            isPlayerInZone = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
    }
}