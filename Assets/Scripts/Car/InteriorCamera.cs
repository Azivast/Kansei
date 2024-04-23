using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteriorCamera : MonoBehaviour
{
    [SerializeField] private float maxRotationAngle = 1f;
    [SerializeField] private InputActionReference look;
    [SerializeField] private Transform forward;
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
    [SerializeField] private Transform back;
    private Rigidbody carRB;
    private Vector3 initialRotation;
    Quaternion targetRotation;


    private void Awake()
    {
        carRB = GetComponentInParent<Rigidbody>();

    }
    
    private void Start()
    {
        initialRotation = transform.forward;
        transform.position = forward.position;
        transform.rotation = forward.rotation;
    }

    private void OnEnable()
    {
        look.action.Enable(); 
        look.action.performed += OnLook;

    }

    private void OnDisable()
    {
        look.action.Disable();
        look.action.performed -= OnLook;

    }

    void Update()
    {
        if (transform.position != forward.position) return; // no time to fix rotation for other directions, only apply to forward
        
        if (carRB.velocity.sqrMagnitude > 0.1f)
        {
            targetRotation = Quaternion.LookRotation(carRB.velocity);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(initialRotation);
        }
       
            
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            angle = Mathf.Clamp(angle, 0f, maxRotationAngle);
            
            Quaternion limitedRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angle);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, limitedRotation, Time.deltaTime);
    }
    
    private void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
        if (context.ReadValue<Vector2>().x > 0.5f && context.ReadValue<Vector2>().x <= 1f)
        {
            transform.position = right.position;
            transform.rotation = right.rotation;
        }
        else if (context.ReadValue<Vector2>().x < -0.5f && context.ReadValue<Vector2>().x >= -1f)
        {
            transform.position = left.position;
            transform.rotation = left.rotation;
        }
        else if (context.ReadValue<Vector2>().y < -0.5f && context.ReadValue<Vector2>().y >= -1f)
        {
            transform.position = back.position;
            transform.rotation = back.rotation;
        }
        else
        {
            transform.position = forward.position;
            transform.rotation = forward.rotation;
        }
    }
}
