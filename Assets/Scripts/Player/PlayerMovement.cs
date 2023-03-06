using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controller moves player in 8 directions
public class PlayerMovement : MonoBehaviour
{
    public float velocity = 5;
    public float turnSpeed = 1;

    float angle;

    Quaternion targetRotation;

    private Vector2 _movement = new Vector2();
    private InputManager _inputManager;

    private DialogueManager _dialogueManager;

    private Animator _animator;

    void Start(){
        _inputManager       = InputManager.Instance;
        _dialogueManager    = DialogueManager.Instance;
        _animator = GetComponent<Animator>();
    }

    void Update(){
        GetInput();
        CalculateDir();
        if(_movement != Vector2.zero) _animator.SetBool("IsWalking", true);
        if(_movement == Vector2.zero) _animator.SetBool("IsWalking", false);
    }

    private void FixedUpdate() {
        if(_dialogueManager.isOpen) return;
        if (_movement == Vector2.zero) return;
        Rotate();
        Move();
    }

    void GetInput() => _movement = _inputManager.GetPlayerMovement();

    //void OnMovement() => _animator.SetBool("IsWalking", true);
    //calculate angle of movement
    void CalculateDir() => angle = ((Mathf.Atan2(_movement.x, _movement.y)) * (Mathf.Rad2Deg));


    //Rotate toward angle of movement
    void Rotate(){
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime*turnSpeed);
    }

    void Move(){
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
}
