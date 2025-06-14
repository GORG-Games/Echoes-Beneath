using UnityEngine;
using Utils;

public class SpikeTrigger : MonoBehaviour
{
    [SerializeField] private Animator _spikeAnimator;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _raiseClip;
    [SerializeField] private AudioClip _lowerClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject))
        {
            _spikeAnimator.SetBool("isRaised", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (LayerMaskUtil.ContainsLayer(_playerLayer, other.gameObject))
        {
            _spikeAnimator.SetBool("isRaised", false);
        }
    }
}