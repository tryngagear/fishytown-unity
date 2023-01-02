using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    private Camera camPos;

    private void Start() {
        camPos = Camera.main;    
    }

    private void LateUpdate() {
        transform.LookAt(camPos.transform);

        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);    
    }
}
