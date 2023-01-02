using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRange = 0.5f;
    [SerializeField] private LayerMask _interactionMask;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    public DialogueUI DialogueUI => dialogueUI;

    private void Update(){
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRange, _colliders
            , _interactionMask);
        if(DialogueUI.isOpen) return;
        if(_numFound > 0){
            var interactable = _colliders[0].GetComponent<IInteractable>();
            if (interactable != null && Keyboard.current.eKey.wasPressedThisFrame){
                interactable.Interact(this);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRange); 
    }


}
