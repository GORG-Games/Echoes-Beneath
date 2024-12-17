using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    // Метод для воспроизведения звука с заданной группой
    public void PlaySound(AudioSource source, AudioClip clip, AudioMixerGroup mixerGroup)
    {
        source.outputAudioMixerGroup = mixerGroup;
        source.PlayOneShot(clip);
    }
}
