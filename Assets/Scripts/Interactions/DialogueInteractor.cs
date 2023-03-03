using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public float speed = 1;
    public float rotationMod;
    public string InteractionPrompt => _prompt;
    public bool Interact(InteractionController interactor){
        Debug.Log("chat");
       /*Vector3 lookPos = interactor.transform.position - transform.position;
        float angle = ((Mathf.Atan2(lookPos.y, lookPos.x)) * (Mathf.Rad2Deg)) - rotationMod;
        Quaternion q = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime*speed);
        */
        transform.LookAt(interactor.transform);
        return true;
    }
}
