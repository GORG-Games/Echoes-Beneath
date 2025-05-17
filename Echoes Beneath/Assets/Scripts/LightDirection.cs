using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDirection : MonoBehaviour
{
    [SerializeField] private PlayerAim _playerAim;
    private float _angle;

    void Update()
    {
        Debug.Log($"LightDirection: angle = {_playerAim.angle}");
        _angle = _playerAim.angle;
        transform.rotation = Quaternion.Euler(0, 0, _angle - 90); // установка угла
    }
}