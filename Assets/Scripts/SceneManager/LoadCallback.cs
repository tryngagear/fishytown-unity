using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCallback : MonoBehaviour
{
    private bool _isFirstUpdate = true;

    private void Update() {
        if(_isFirstUpdate){
            _isFirstUpdate = false;
            SceneLoader.LoadCallback();
        }
    }
}
