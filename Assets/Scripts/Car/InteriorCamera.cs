using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorCamera : MonoBehaviour
{
    [SerializeField] private float maxRotationAngle = 1f;
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
    }
    
    void Update()
    {
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
}
