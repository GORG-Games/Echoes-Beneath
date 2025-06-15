using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSoundDesign : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip clip;
    public void PlayUISound()
    {
        source.PlayOneShot(clip);
    }
}