using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class MenuCamera : MonoBehaviour
{
    //script to move the cam so that teh player can move between menu scenes

    private Vector3 startPosition;
    private Vector3 desiredPosition;

    private Quaternion startRotation;
    private Quaternion desiredRotation;
    
    public Transform shopWayPoint;
    public Transform levelWayPoint;

    private Vector3 axis = Vector3.up;
    public float rate;
    public AnimationCurve curve;

    private void Start()
    {
        startPosition = desiredPosition = transform.localPosition;
        startRotation = desiredRotation = transform.rotation;
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPosition, 0.1f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, 0.1f);
    }

    public void BackToMainMenu()
    {
        Debug.Log("BackToMainMenu");
        desiredPosition = startPosition;
        desiredRotation = startRotation;
    }

    public void MoveToShop()
    {
        Debug.Log("MoveToShop");
        desiredPosition = shopWayPoint.localPosition;
        desiredRotation = shopWayPoint.localRotation;
    }

    public void MoveToLevel()
    {
        Debug.Log("MoveToLevel");
        desiredPosition = levelWayPoint.localPosition;
        desiredRotation = levelWayPoint.localRotation;
    }
}
