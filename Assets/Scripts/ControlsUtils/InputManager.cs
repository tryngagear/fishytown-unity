using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private PlayerControls playerControls;

    protected override void Awake(){
        base.Awake();
        playerControls = new PlayerControls();
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement(){
        return playerControls.PlayerActMap.Movement.ReadValue<Vector2>();
    }

    public bool GetPlayerInteract(){
        return playerControls.PlayerActMap.Interact.triggered;
    }
/*
    public bool PlayerMovementIsPressed(){
        return playerControls.PlayerActMap.Movement.ReadValueAsButton()
    }
*/}
