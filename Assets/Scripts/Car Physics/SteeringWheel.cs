using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    [SerializeField] private int maxSteeringWheelAngle = 900;
    private CarController controller;
    private Vector3 initialRotation;

    private void Awake()
    {
        controller = GetComponentInParent<CarController>();
    }

    private void Start()
    {
        initialRotation = transform.localEulerAngles;
    }

    void Update()
    {
        float steerFraction = controller.SteeringAngle / controller.MaxSteeringAngle;
        Debug.Log(steerFraction);
        
        transform.localRotation = Quaternion.Euler(
            initialRotation.x, 
            initialRotation.y, 
            initialRotation.z + steerFraction * maxSteeringWheelAngle);
        

    }
}
                