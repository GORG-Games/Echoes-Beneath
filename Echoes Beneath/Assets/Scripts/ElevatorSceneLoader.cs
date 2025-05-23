using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorSceneLoader : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;
    private bool isPlayerInZone = false;
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] Shotgun shotgun;
    [SerializeField] PlayerHealth playerHealth;

    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;
    void Start()
    {
            PlayerPrefs.DeleteKey("TotalAmmo");
            PlayerPrefs.DeleteKey("MedkitCount");
            PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            PlayerPrefs.SetInt("TotalAmmo", shotgun._totalAmmo);
            PlayerPrefs.SetInt("MedkitCount", playerHealth.MedkitCount);
            PlayerPrefs.Save();
            _sceneLoader.LoadNextScene();
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
