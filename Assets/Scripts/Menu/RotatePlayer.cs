using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class RotatePlayer : MonoBehaviour
{
public float degreesPerSecond = 30;
    private void Update()
    {
        transform.Rotate(new Vector3(0, degreesPerSecond , 0) * Time.deltaTime);
    }
}
