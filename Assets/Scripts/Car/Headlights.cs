using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Headlights : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleHeadlightsInput;
    [SerializeField] private GameObject lights;
    [SerializeField] private float animationSpeed = 3f;
    [SerializeField] private float rotationOpen= 211.324f;
    [SerializeField] private float rotationClosed= 270.401f;
    private Vector3 rotationCurrent;
    private float rotationTarget;
    private bool open;

    private void Start()
    {
        rotationCurrent = transform.localEulerAngles;
        rotationTarget = rotationOpen;
        open = true;
    }

    private void OnEnable()
    {
        if (toggleHeadlightsInput == null) return;
        toggleHeadlightsInput.action.Enable();
        toggleHeadlightsInput.action.performed += OnToggleHeadlightsInput;
    }

    private void OnDisable()
    {
        if (toggleHeadlightsInput == null) return;
        toggleHeadlightsInput.action.Disable();
        toggleHeadlightsInput.action.performed -= OnToggleHeadlightsInput;
    }

    private void OnToggleHeadlightsInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleHeadlights();
        }
    }

    private void ToggleHeadlights()
    {
        if (open)
        {
            open = false;
            rotationTarget = rotationClosed;
            lights.SetActive(false);
        }
        else
        {
            open = true;
            rotationTarget = rotationOpen;
            lights.SetActive(true);
        }
    }
    
    private void Update()
    {
        
        
        rotationCurrent.x = Mathf.LerpAngle(rotationCurrent.x, rotationTarget, Time.deltaTime * animationSpeed);
        //rotationCurrent.x = rotationTarget;
        transform.localEulerAngles = rotationCurrent;
    }
}
