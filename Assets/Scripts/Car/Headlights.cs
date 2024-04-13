using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Headlights : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleHeadlightsInput;
    [SerializeField] private GameObject lights;
    [SerializeField] private float animationDuration = 1.0f;
    [SerializeField] private float rotationAmount = 90f;
    [SerializeField] private bool startOn = false;
    private Vector3 closed;
    private Vector3 open;
    private Vector3 target;

    private void Start()
    {
        open = transform.localEulerAngles;
        closed = open - new Vector3(rotationAmount, 0, 0);
    }

    private void OnEnable()
    {
        toggleHeadlightsInput.action.Enable();
        toggleHeadlightsInput.action.performed += OnToggleHeadlightsInput;
    }

    private void OnDisable()
    {
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
        if (lights.activeSelf)
        {
            lights.SetActive(false);
            target = closed;
            Rotate();
        }
        else
        {
            target = open;
            Rotate();
            lights.SetActive(true);
        }
    }
    
    // coroutine that lerps the transform n degrees in n seconds
    private void Rotate()
    {
        transform.localEulerAngles = target; //todo: lerp
    }
}
