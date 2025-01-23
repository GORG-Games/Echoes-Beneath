using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDirection : MonoBehaviour
{
    [SerializeField] private PlayerAim _playerAim;
    private float _angle;

    void Update()
    {
        _angle = _playerAim.angle;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, _angle - 90));
    }
}