using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Suspension : MonoBehaviour
{
    [SerializeField] private Rigidbody carRB;

    [SerializeField] private InputActionReference throttleInput;

    [Header("Spring")]
    public float unloadedLength;
    public float compressedLength;
    public float springStiffness;
    
    [Header("Damper")]
    public float damperStiffness;

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float currentSpringLength;
    private float springForce;
    private float springVelocity;
    private float damperForce;

    [Header("Wheel")]
    public float wheelRadius;
    public float wheelAngle;
    public float steerAngle;
    public float steerTime;
    public Transform wheelTransform;

    public float sidewaysGrip;
    public float acceleration;
    [HideInInspector] public bool WheelOnGround;

    private Vector3 suspensionForce;
    private Vector3 localWheelVelocity;
    private Vector2 wheelForce;

    private void OnEnable() 
    {
        throttleInput.action.Enable();
    }

    private void Start()
    {
        minLength = compressedLength;
        maxLength = unloadedLength;

        wheelTransform = transform.GetChild(0).transform;
    }

    private void Update()
    {
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, steerTime * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(
            transform.localRotation.x,
            transform.localRotation.y + wheelAngle,
            transform.localRotation.z);

        Debug.DrawRay(transform.position, -transform.up * currentSpringLength, Color.yellow);
    }

    private void FixedUpdate() 
    {
        float rayLength = unloadedLength+wheelRadius;
        if (Physics.Raycast(transform.position-transform.up*wheelRadius, -transform.up, out RaycastHit hit, rayLength))
        {
            WheelOnGround = true;
            lastLength = currentSpringLength;
            currentSpringLength = hit.distance - wheelRadius;
            currentSpringLength = Mathf.Clamp(currentSpringLength, minLength, maxLength);
            springVelocity = (lastLength-currentSpringLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (unloadedLength - currentSpringLength);
            damperForce = damperStiffness * springVelocity;

            wheelTransform.transform.position = hit.point + transform.up*wheelRadius;

            suspensionForce = (springForce+damperForce)*carRB.transform.up;

            localWheelVelocity = transform.InverseTransformDirection(carRB.GetPointVelocity(hit.point));
            wheelForce.x = throttleInput.action.ReadValue<float>() * springForce * sidewaysGrip;
            wheelForce.y = localWheelVelocity.x * springForce*acceleration;

            carRB.AddForceAtPosition(suspensionForce + 
            (wheelForce.x*transform.forward) + 
            (wheelForce.y*-transform.right), 
            hit.point);

        }
        else
        {
            WheelOnGround = false;
            wheelTransform.transform.position = transform.position -transform.up*rayLength; // lerp thowards full extended
    
        }
    }
}
