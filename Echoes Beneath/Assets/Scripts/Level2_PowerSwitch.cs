using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2_PowerSwitch : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;

    [Header("Changing Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite activatedSwitch;

    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private GameObject powerOnText;
    [SerializeField] private GameObject powerOnText2;

    private bool isPlayerInZone = false;
    private bool hasActivated = false;

    [Header("Affected by Activation Properties")]
    [SerializeField] private GameObject elevator;
    private Level2_ElevatorSwitch elevatorScript;
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private ParticleSystem particleEffect;
    private Coroutine _coroutine;
    void Start()
    {
        if (elevator != null) elevatorScript = elevator.GetComponent<Level2_ElevatorSwitch>();
    }

    void Update()
    {
        if (isPlayerInZone && !hasActivated && Input.GetKeyDown(KeyCode.E))
        {
            ActivateSwitch();
        }
    }

    void ActivateSwitch()
    {
        elevatorScript.isPowered = true;
        ChangeSprite();
        StartParticles();
        ActivateObjects();
        hasActivated = true;
        _coroutine = StartCoroutine(PowerOnTextDisplay());

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    void ChangeSprite()
    {
        if (spriteRenderer != null && activatedSwitch != null)
        {
            spriteRenderer.sprite = activatedSwitch;
        }
    }
    void ActivateObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }
    void StartParticles()
    {
        particleEffect.Play();
    }
    IEnumerator PowerOnTextDisplay()
    {
        powerOnText.SetActive(true);

        yield return new WaitForSeconds(2f);

        powerOnText.SetActive(false);

        powerOnText2.SetActive(true);

        yield return new WaitForSeconds(2f);

        powerOnText2.SetActive(false);

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
