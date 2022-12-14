using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class MenuCamera : MonoBehaviour
{
    //script to move the cam so that teh player can move between menu scenes

    private Vector3 startPosition;
    private Vector3 desiredPosition = new Vector3(0f,0f,0f);

    private Quaternion startRotation;
    private Quaternion desiredRotation;
    
    public Transform shopWayPoint;
    public Transform levelWayPoint;

    private Vector3 axis = Vector3.up;
    public AnimationCurve curve;

    public float CameraLerp;

    private void Start()
    {
        Debug.Log("MenuCamera Start Called");
        startPosition = transform.localPosition;
        startRotation = transform.rotation;
        if (desiredPosition.x == 0) // on scene load, set camera to mainMenu position
        {
            BackToMainMenu();
        }
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPosition, CameraLerp);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, CameraLerp);
    }

    public void BackToMainMenu()
    {
        Debug.Log("MenuCamera.BackToMainMenu");
        desiredPosition = startPosition;
        desiredRotation = startRotation;
    }

    public void MoveToShop()
    {
        Debug.Log("MenuCamera.MoveToShop");
        desiredPosition = shopWayPoint.localPosition;
        desiredRotation = shopWayPoint.localRotation;
    }

    public void MoveToLevel()
    {
        Debug.Log("MenuCamera.MoveToLevel");
        desiredPosition = levelWayPoint.localPosition;
        desiredRotation = levelWayPoint.localRotation;
    }
}
