using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controller moves player in 8 directions
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    public float velocity = 5;
    public float turnSpeed = 1;

    Vector2 input;
    float angle;

    Quaternion targetRotation;
    Transform cam;        

    public DialogueUI DialogueUI => dialogueUI;

    void Start(){
        cam = Camera.main.transform;
    }

    void Update(){
        GetInput();

        if(DialogueUI.isOpen) return;
        if ((input.x == 0) && (input.y == 0) ) return;

        CalculateDir();
        Rotate();
        Move();
    }
    //input.x = up and down
    //input.y = left and right
    void GetInput(){
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    //calculate angle of movement
    void CalculateDir(){
        angle = ((Mathf.Atan2(input.x, input.y)) * (Mathf.Rad2Deg)) + cam.eulerAngles.y;
    }

    //Rotate toward angle of movement
    void Rotate(){
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime*turnSpeed);
    }

    void Move(){
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
}
