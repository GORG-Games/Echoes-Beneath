using UnityEngine;
using UnityEngine.Audio;

public class ReverbZoneTrigger : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private float reverbOnEnter = -10f;
    [SerializeField] private float reverbOnExit = -80f;
    private const string PARAM = "ReverbSendVolume";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(playerLayer, other.gameObject))
        {
            mixer.SetFloat(PARAM, reverbOnEnter);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(playerLayer, other.gameObject))
        {
            mixer.SetFloat(PARAM, reverbOnExit);
        }
    }
}