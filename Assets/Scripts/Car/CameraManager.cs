using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera[] cameras;
    [SerializeField] private InputActionReference changeCameraInput;

    private void OnEnable()
    {
        changeCameraInput.action.Enable();
        changeCameraInput.action.performed += OnChangeCameraInput;
        
        // Disable all cameras except the first one
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        changeCameraInput.action.performed -= OnChangeCameraInput;
    }

    private void NextCamera()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].enabled)
            {
                cameras[i].enabled = false;
                cameras[(i + 1) % cameras.Length].enabled = true;
                return;
            }
        }
    }
    
    private void OnChangeCameraInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            NextCamera();
        }
    }
}
