using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private Suspensions suspension;

    [Header("Car Specs")]
    public float wheelBase;
    public float rearTrack;
    public float turnRadius;

    [Header("Inputs")]
    public float steerAmount;

    private float ackermannAngleLeft;
    private float ackermannAngleRight;

    private void OnEnable()
    {
        steerInput.action.Enable();
    }


    public void Update() 
    {
        steerAmount = steerInput.action.ReadValue<float>();
        Debug.Log(steerAmount);
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


