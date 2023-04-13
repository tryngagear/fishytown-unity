using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    public GameObject _UI;
    //private bool _isOpen = false;
    public PlayerControls playerControls;
    private InputAction _menuOpen;
    protected override void Awake(){
        base.Awake();
        playerControls = new PlayerControls();
    }

    private void OnEnable() {
    //    _menuOpen = playerControls.PlayerActMap.OpenMenu;
        playerControls.Enable();
    //    playerControls.PlayerActMap.OpenMenu.performed += OnOpenMenu;
    //    playerControls.PlayerActMap.OpenMenu.Enable();
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

    public bool GetPlayerMenuOpen(){
        return playerControls.PlayerActMap.OpenMenu.triggered;
    }
/*
    public void OnOpenMenu(InputAction.CallbackContext obj){
        _isOpen = !_isOpen;
        _UI.SetActive(_isOpen);
        Time.timeScale = _isOpen ? 0:1;
    }
*/
/*
    public bool PlayerMovementIsPressed(){
        return playerControls.PlayerActMap.Movement.ReadValueAsButton()
    }
*/}
