using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSceneLoader2 : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;
    private bool isPlayerInZone = false;
    [SerializeField] private SceneLoader _sceneLoader;

    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            _sceneLoader.LoadSceneByIndex(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject))
        {
            isPlayerInZone = true;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject))
        {

            isPlayerInZone = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
    }
}
