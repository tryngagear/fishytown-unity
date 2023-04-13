using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
    public GameObject _UI;
    private bool _isOpen = false;
    private InputManager _inputManager;

//    private PlayerControls _playerControls;
    private InputAction _menuOpen;
    protected override void Awake() {
        base.Awake();
        _inputManager   = InputManager.Instance;
        _UI.SetActive(false);
    }

    private void OnEnable() {
        _menuOpen = _inputManager.playerControls.PlayerActMap.OpenMenu;
        _menuOpen.performed += OnOpenMenu;
        _menuOpen.Enable();
    }

     public void OnOpenMenu(InputAction.CallbackContext obj){
        _isOpen = !_isOpen;
        _UI.SetActive(_isOpen);
        Time.timeScale = _isOpen ? 0:1;
    }
}
