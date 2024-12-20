using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionManipulator : MonoBehaviour
{
    [Header("������ ������ (���� ����� ������")]
    public float leftCameraSize;
    [Header("������ ������ (���� ����� �������")]
    public float rightCameraSize;
    [Header("������ ������ (���� ����� �����")]
    public float upCameraSize;
    [Header("������ ������ (���� ����� ����")]
    public float downCameraSize;


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.transform.position.x > transform.position.x) //right
            {
                CameraController.changeCameraSizeEvent?.Invoke(rightCameraSize);
            }
            else if (other.transform.position.x < transform.position.x)//left
            {
                CameraController.changeCameraSizeEvent?.Invoke(leftCameraSize);
            }
            /*else if (other.transform.position.y > transform.position.y)//up
            {
                CamController.changeCameraSizeEvent?.Invoke(upCameraSize);
            }
            else if (other.transform.position.y < transform.position.y)//down
            {
                CamController.changeCameraSizeEvent?.Invoke(downCameraSize);
            }*/
        }
    }
}