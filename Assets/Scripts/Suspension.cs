using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform wheelModel;

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

    private Vector3 suspensionForce;


    void Start()
    {
        minLength = compressedLength;
        maxLength = unloadedLength;

        wheelModel = transform.GetChild(0).transform;
    }

    private void FixedUpdate() {
        float rayLength = unloadedLength+wheelRadius;
        if (Physics.Raycast(transform.position-transform.up*wheelRadius, -transform.up, out RaycastHit hit, rayLength))
        {
            lastLength = currentSpringLength;
            currentSpringLength = hit.distance - wheelRadius;
            currentSpringLength = Mathf.Clamp(currentSpringLength, minLength, maxLength);
            springVelocity = (lastLength-currentSpringLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (unloadedLength - currentSpringLength);
            damperForce = damperStiffness * springVelocity;

            wheelModel.transform.position = hit.point + transform.up*wheelRadius;

            suspensionForce = (springForce+damperForce)*carRB.transform.up;
            carRB.AddForceAtPosition(suspensionForce, hit.point);

        }
        else
        {
            Debug.Log($"not on gound, {rayLength}");
            wheelModel.transform.position = transform.position -transform.up*rayLength; // lerp thowards full extended
            Debug.Log(transform.position -transform.up*rayLength);
        }
    }
}
