using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CarController : MonoBehaviour
{
    [Header("Input")]
    public bool AIControlled = false;
    public InputActionReference steerInput; 
    public InputActionReference throttleInput; 
    public InputActionReference brakeInput; 
    public InputActionReference handbrakeInput; 

    [Header("Car Stats")] 
    [SerializeField] private float engineTorque;
    [SerializeField, Tooltip("Maximum brake torque Nm")] private float brakeTorque;
    [SerializeField, Range(0, 1), Tooltip("Bias towards front brakes")] private float brakeBias;
    [SerializeField, Tooltip("Maximum brake torque Nm")] private float handbrakeTorque;
    [SerializeField] private float maxSteeringAngle = 60;
    [SerializeField] private float maxSpeed;

    [Header("Settings")]
    [SerializeField] private AnimationCurve steeringCurve;
    [SerializeField] private float slipSmokeThreshold = 0.3f;
    public float steerTime = 8f;
    
    [Header("Components")]
    [SerializeField] private Wheels Wheels;
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private GameObject tireSmokePrefab;
    
    //[Header("Variables")
    [SerializeField] private float throttleAmount;
    [SerializeField] private float brakeAmount;
    [SerializeField] private float handbrakeAmount;
    [SerializeField] private float steerAmount;

    [HideInInspector] public float SteeringAngle;
    private float speed;
    private float speedClamped;
    private float slipAngle;

    public float MaxSteeringAngle => maxSteeringAngle;
    public float Speed => speed;

    public float SpeedRatio
    {
        get
        {
            var throttleClamped = Mathf.Clamp(Mathf.Abs(throttleAmount), 0.5f, 1f); // todo explain the reason for this
            return speedClamped*throttleClamped/maxSpeed;
        }
    }
        
    private void Awake()
    {
        carRB = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        steerInput.action.Enable();
        throttleInput.action.Enable();
        brakeInput.action.Enable();
        handbrakeInput.action.Enable();
    }

    private void Start()
    {
        InstantiateTireSmoke();
    }

    private void InstantiateTireSmoke()
    {
        Wheels.FrontLeft.Smoke = Instantiate(tireSmokePrefab, 
            Wheels.FrontLeft.Mesh.transform.position - Wheels.FrontLeft.Mesh.transform.up * Wheels.FrontLeft.Collider.radius, 
            Wheels.FrontLeft.Collider.transform.rotation, 
            Wheels.FrontLeft.Collider.transform).GetComponent<ParticleSystem>();

        Wheels.FrontRight.Smoke = Instantiate(tireSmokePrefab, 
            Wheels.FrontRight.Mesh.transform.position - Wheels.FrontRight.Mesh.transform.up * Wheels.FrontRight.Collider.radius, 
            Wheels.FrontRight.Collider.transform.rotation, 
            Wheels.FrontRight.Collider.transform).GetComponent<ParticleSystem>();

        Wheels.RearLeft.Smoke = Instantiate(tireSmokePrefab, 
            Wheels.RearLeft.Mesh.transform.position - Wheels.RearLeft.Mesh.transform.up * Wheels.RearLeft.Collider.radius, 
            Wheels.RearLeft.Collider.transform.rotation, 
            Wheels.RearLeft.Collider.transform).GetComponent<ParticleSystem>();

        Wheels.RearRight.Smoke = Instantiate(tireSmokePrefab, 
            Wheels.RearRight.Mesh.transform.position - Wheels.RearRight.Mesh.transform.up * Wheels.RearRight.Collider.radius, 
            Wheels.RearRight.Collider.transform.rotation, 
            Wheels.RearRight.Collider.transform).GetComponent<ParticleSystem>();

    }

    public void SetInputs(float steer, float throttle, float brake, float handbrake)
    {
        steerAmount = steer;
        throttleAmount = throttle;
        brakeAmount = brake;
        handbrakeAmount = handbrake;
    }

    private void GetPlayerInput()
    {
        steerAmount = steerInput.action.ReadValue<float>(); 
        throttleAmount = throttleInput.action.ReadValue<float>();
        brakeAmount = brakeInput.action.ReadValue<float>();
        handbrakeAmount = handbrakeInput.action.ReadValue<float>();
    }

    private void UpdateWheelPosition(WheelCollider collider, MeshRenderer mesh)
    {
        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        mesh.transform.position = position;
        mesh.transform.rotation = rotation;
    }

    private void UpdateAllWheelsPositions()
    {
        UpdateWheelPosition(Wheels.FrontLeft.Collider, Wheels.FrontLeft.Mesh);
        UpdateWheelPosition(Wheels.FrontRight.Collider, Wheels.FrontRight.Mesh);
        UpdateWheelPosition(Wheels.RearLeft.Collider, Wheels.RearLeft.Mesh);
        UpdateWheelPosition(Wheels.RearRight.Collider, Wheels.RearRight.Mesh);
    }

    private void EvaluateTireSmoke()
    {
        for (int i = 0; i < 4; i++)
        {
            Wheels.AsArray[i].Collider.GetGroundHit(out WheelHit hit);
            if (Mathf.Abs(hit.sidewaysSlip)+Mathf.Abs(hit.forwardSlip) > slipSmokeThreshold)
            {
                if (!Wheels.AsArray[i].Smoke.isPlaying) Wheels.AsArray[i].Smoke.Play(); //todo: optimize
            }
            else
            {
                Wheels.AsArray[i].Smoke.Stop();
            }
        }
    }

    private void ApplyAcceleration()
    {
        if (speed > maxSpeed)
        {
            Wheels.RearLeft.Collider.motorTorque = 0;
            Wheels.RearRight.Collider.motorTorque = 0;
            return;
        }
        Wheels.RearLeft.Collider.motorTorque = engineTorque * throttleAmount;
        Wheels.RearRight.Collider.motorTorque = engineTorque * throttleAmount;
    }
    
    private void ApplySteering()
    {
        //todo: Ackermann steering?
        
        var requestedSteeringAngle = steeringCurve.Evaluate(speed)*steerAmount*maxSteeringAngle;
        
        SteeringAngle = Mathf.Lerp(SteeringAngle, requestedSteeringAngle, steerTime * Time.fixedDeltaTime);

        
        Wheels.FrontLeft.Collider.steerAngle = SteeringAngle;
        Wheels.FrontRight.Collider.steerAngle = SteeringAngle;
        
        slipAngle = Vector3.Angle(transform.forward, carRB.velocity); // removed -transform.forward. Needed?
    }

    private void ApplyBrakes()
    {
        //todo: reverse if moving backwards or standing still
        
        Wheels.FrontLeft.Collider.brakeTorque = brakeAmount * brakeTorque * brakeBias;
        Wheels.FrontRight.Collider.brakeTorque = brakeAmount * brakeTorque * brakeBias;
        
        // Factor in handbrake. Assuming disk brakes for footbrake and drum brakes for handbrake => 2 separate systems, don't clamp total value
        float rearBrakeForce = brakeAmount * brakeTorque * (1 - brakeBias) + handbrakeAmount * handbrakeTorque;

        Wheels.RearLeft.Collider.brakeTorque = rearBrakeForce;
        Wheels.RearRight.Collider.brakeTorque = rearBrakeForce;
    }

    private void Update()
    {
        EvaluateTireSmoke();
        UpdateAllWheelsPositions();
    }

    private void FixedUpdate()
    {
        speed = Wheels.RearLeft.Collider.rpm * Wheels.RearLeft.Collider.radius * 2 * Mathf.PI / 10;
        speedClamped = Mathf.Lerp(speedClamped, speed, Time.fixedDeltaTime);
        
        if (!AIControlled) GetPlayerInput();
        ApplySteering();
        ApplyAcceleration();
        ApplyBrakes();
    }

    public void ResetMovement()
    {
        carRB.velocity = Vector3.zero;
        carRB.angularVelocity = Vector3.zero;

        foreach (Wheels.Wheel wheel in Wheels.AsArray)
        {
            wheel.Collider.brakeTorque = 0;
            wheel.Collider.motorTorque = 0;
            wheel.Collider.steerAngle = 0;
        }
    }
}

[Serializable]
public class Wheels
{
    [Serializable]
    public class Wheel
    {
        public WheelCollider Collider;
        public MeshRenderer Mesh;
        [HideInInspector] public ParticleSystem Smoke;
    }
    
    public Wheel FrontLeft;
    public Wheel FrontRight;
    public Wheel RearLeft;
    public Wheel RearRight;
    
    public Wheel[] AsArray => new[] {FrontLeft, FrontRight, RearLeft, RearRight};
}