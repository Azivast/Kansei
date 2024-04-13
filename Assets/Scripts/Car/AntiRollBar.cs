using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    [SerializeField] private WheelCollider LeftWheel;
    [SerializeField] private WheelCollider RightWheel;
    [SerializeField] private float RollBarStiffness;
    private Rigidbody carRB;

    private void Awake()
    {
        carRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

        float leftTravel = 1.0f, rightTravel = 1.0f;
        WheelHit hit;
        
        // Calc the travel of the wheels
        bool LeftOnGround = LeftWheel.GetGroundHit(out hit);
        if (LeftOnGround) leftTravel = (-LeftWheel.transform.InverseTransformPoint(hit.point).y - LeftWheel.radius) / LeftWheel.suspensionDistance;
        bool RightOnGround = RightWheel.GetGroundHit(out hit);
        if (RightOnGround) rightTravel = (-RightWheel.transform.InverseTransformPoint(hit.point).y - RightWheel.radius) / RightWheel.suspensionDistance;

        // Apply the anti-roll force
        float antiRollForce = (leftTravel - rightTravel) * RollBarStiffness;
        if (LeftOnGround) carRB.AddForceAtPosition(LeftWheel.transform.up * -antiRollForce, LeftWheel.transform.position);
        if (RightOnGround) carRB.AddForceAtPosition(RightWheel.transform.up * antiRollForce, RightWheel.transform.position);
    }
}
