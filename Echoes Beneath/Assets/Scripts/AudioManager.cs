using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    // ����� ��� ��������������� ����� � �������� �������
    public void PlaySound(AudioSource source, AudioClip clip, AudioMixerGroup mixerGroup)
    {
        source.outputAudioMixerGroup = mixerGroup;
        source.PlayOneShot(clip);
    }
}
