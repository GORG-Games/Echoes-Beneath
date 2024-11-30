using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionManipulator : MonoBehaviour
{
    [Header("Размер камеры (если пойти налево")]
    public float leftCameraSize;
    [Header("Размер камеры (если пойти направо")]
    public float rightCameraSize;
    [Header("Размер камеры (если пойти вверх")]
    public float upCameraSize;
    [Header("Размер камеры (если пойти вниз")]
    public float downCameraSize;


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.transform.position.x > transform.position.x) //Если пошёл направо
            {
                CameraController.changeCameraSizeEvent?.Invoke(rightCameraSize);
            }
            else if (other.transform.position.x < transform.position.x)//Если пошёл налево
            {
                CameraController.changeCameraSizeEvent?.Invoke(leftCameraSize);
            }
            /*else if (other.transform.position.y > transform.position.y)//Если пошёл вверх
            {
                CamController.changeCameraSizeEvent?.Invoke(upCameraSize);
            }
            else if (other.transform.position.y < transform.position.y)//Если пошёл вниз
            {
                CamController.changeCameraSizeEvent?.Invoke(downCameraSize);
            }*/
        }
    }
}