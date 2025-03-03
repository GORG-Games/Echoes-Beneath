using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] AudioMixer _audioMixer;

    [Header ("References")]
    [SerializeField] private GameObject _pauseMenuObject;
    public bool _isPaused;

    [Header("Player Components to Disable")]
    [Tooltip("Drag here player scripts for moving shooting and aiming")]
    [SerializeField] private MonoBehaviour[] playerComponents; // Компоненты игрока, которые будут отключены при паузе
    void Start()
    {
        _pauseMenuObject.SetActive(false);
        _isPaused = false;
    }
    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
/*#if UNITY_EDITOR
            Debug.Log("Escape pressed");
#endif*/
            if (!_isPaused)
            {
                OpenMenu();
            }
            else if (_isPaused)
            {
                CloseMenu();
            }
        }
    }
    private void OpenMenu()
    {
        _pauseMenuObject.SetActive(true);
        _isPaused = true;
        foreach (MonoBehaviour comp in playerComponents)
        {
            if (comp != null)
                comp.enabled = false;
        }
    }
    private void CloseMenu()
    {
        _pauseMenuObject.SetActive(false);
        _isPaused = false;
        foreach (MonoBehaviour comp in playerComponents)
        {
            if (comp != null)
                comp.enabled = true;
        }
    }

}
