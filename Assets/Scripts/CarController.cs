using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CarController : MonoBehaviour
{
    [Serializable]
    internal class Suspensions
    {
        public Suspension FrontLeft;
        public Suspension FrontRight;
        public Suspension RearLeft;
        public Suspension RearRight;

    }
    public InputActionReference steerInput;
    [SerializeField] private InputActionReference throttleInput;
    [SerializeField] private Suspensions suspension;

    [Header("Car Specs")]
    public float wheelBase;
    public float rearTrack;
    public float turnRadius;

    [Header("Inputs")] 
    public bool aiControlled = false;
    public float steerAmount;
    public float throttle;

    private float ackermannAngleLeft;
    private float ackermannAngleRight;

    private void OnEnable()
    {
        if (!aiControlled)
        {
            steerInput.action.Enable();
            throttleInput.action.Enable();
        }
    }

    public void SetInputs(float steerAmount, float throttle) {
        if (aiControlled)
        {
            this.steerAmount = steerAmount;
            this.throttle = throttle;
        }
        else
        {
            this.steerAmount = steerInput.action.ReadValue<float>();
            this.throttle = throttleInput.action.ReadValue<float>();}
    }

    public void Update() 
    {
        if (!aiControlled) SetInputs(0, 0);
        if (steerAmount > 0) // steering left
        {
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerAmount;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerAmount;
        }
        else if (steerAmount < 0) // steering left
        {
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerAmount;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerAmount;
        }
        else
        {
            ackermannAngleLeft = 0;
            ackermannAngleRight = 0;
        }

        suspension.FrontLeft.steerAngle = ackermannAngleLeft;
        suspension.FrontRight.steerAngle = ackermannAngleRight;
    }
}


