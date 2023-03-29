using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRange = 0.5f;
    [SerializeField] private LayerMask _interactionMask;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private DialogueManager _dialougeManager;
    private InputManager _inputManager;

    private void Start(){
        _dialougeManager = DialogueManager.Instance;
        _inputManager = InputManager.Instance;
    }

    private void Update(){
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRange, _colliders
            , _interactionMask);
        if(_dialougeManager.isOpen) return;
        if(_numFound > 0){
            var interactable = _colliders[0].GetComponent<IInteractable>();
            if (interactable != null && _inputManager.GetPlayerInteract()){
                interactable.Interact(this);
            }
        }
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRange); 
    }


}
