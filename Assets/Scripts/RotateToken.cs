using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class RotateToken : MonoBehaviour
{
public float degreesPerSecond = 60;
    private void Update()
    {
        transform.Rotate(new Vector3(degreesPerSecond, degreesPerSecond , 0) * Time.deltaTime);
    }
}
