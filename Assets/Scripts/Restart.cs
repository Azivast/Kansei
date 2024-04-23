using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    [SerializeField] private InputActionReference restart;
    [SerializeField] private InputActionReference returnToMenu;

    private void OnEnable()
    {
        restart.action.Enable();
        restart.action.performed += OnRestart;
        returnToMenu.action.Enable();
        returnToMenu.action.performed += OnReturnToMenu;
    }

    private void OnDisable()
    {
        restart.action.Disable();
        restart.action.performed -= OnRestart;
        returnToMenu.action.Disable();
        returnToMenu.action.performed -= OnReturnToMenu;
    }
    
    private void OnRestart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene("Touge");
        }
    }
    private void OnReturnToMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
