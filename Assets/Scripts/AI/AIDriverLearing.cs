using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AIDriverLearning : Agent
{
    [SerializeField] private Transform startPos; 
    [SerializeField] private CarController carController;
    [SerializeField] private CheckpointCollection checkpoints;
    [SerializeField] private RayPerceptionSensorComponent3D sideRays;
    private Checkpoint targetCheckpoint;
    private Rigidbody carRB;
    private float timer = 0;
    private float timeTaken;
    private bool finished;
    public float Speed;
    private float speedTimer;

    protected override void OnEnable()
    {
        base.OnEnable();
        carController = GetComponent<CarController>();
        carRB = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        carController.ResetMovement();
        transform.position = startPos.position + 
                             new Vector3(Random.insideUnitCircle.x, 0 ,Random.insideUnitCircle.y);
        transform.rotation = startPos.rotation;
        targetCheckpoint = checkpoints.GetFirstCheckpoint();
        
        timeTaken = 0;
        speedTimer = 0;
        finished = false;
    }
    
    private void FinishEpisode()
    {
        AddReward(1000/timeTaken);
        EndEpisode();
    }

    public float slip;
    public float checkpointDir;
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 driftValue = transform.InverseTransformVector(carRB.velocity);
        slip = (Mathf.Atan2(driftValue.x, driftValue.z) * Mathf.Rad2Deg);
        slip *=2;
        
        Vector3 checkpointValue = transform.InverseTransformVector(targetCheckpoint.transform.forward);
        checkpointDir = (Mathf.Atan2(checkpointValue.x, checkpointValue.z) * Mathf.Rad2Deg);
        checkpointDir *= 2;
        
        sensor.AddObservation(carRB.velocity);
        sensor.AddObservation(slip);
        sensor.AddObservation(checkpointDir);
        
        Speed = carRB.velocity.magnitude;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float steer = actions.ContinuousActions[0];
        float throttle = Mathf.Clamp(actions.ContinuousActions[1], 0, 1);
        float brake = -Mathf.Clamp(actions.ContinuousActions[1], -1, 0);
        
        carController.SetInputs(steer, throttle, brake, 0);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = carController.steerInput.action.ReadValue<float>(); 
        float throttleAndBrake = carController.throttleInput.action.ReadValue<float>() - carController.brakeInput.action.ReadValue<float>();
        actions[1] = throttleAndBrake;

        // Since ContinuousActions range from -1 to 1 and they are converted to 0-1 in OnActionReceived,
        // the input value must be converted to -1 to 1.
        // i = 1 skip steer
        for (int i = 1; i < actions.Length; i++)
        {
            actions[i] = (actions[i] - 0.5f) * 2;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out _))
        {
            AddReward(+30);
            finished = true;
            return;
        }
        
        if (other.TryGetComponent<Checkpoint>(out Checkpoint thisCheckpoint))
        {
            if (thisCheckpoint == targetCheckpoint)
            {
                targetCheckpoint = checkpoints.GetNextCheckpoint(targetCheckpoint);
                AddReward(+10);
            }
            else
            {
                SetReward(0);
                Debug.Log("Car drove wrong way");
                EndEpisode();
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent<Wall>(out Wall wall))
        {
            if (wall.HasSpecialPenalty)
            {
                AddReward(-wall.SpecialPenalty);
            }
            else
            {
                //AddReward(-6f);
                //finished = true;
                EndEpisode();
                //SetReward(0);
            }

        }
    }
    
    private void FixedUpdate()
    {
        //if (Speed < 7f && Speed > 2f && MathF.Abs(slip) < 3f) AddReward(0.2f); // Add reward for not drifting
        timeTaken += Time.fixedDeltaTime;
        
        if (Speed >= 3) AddReward(0.04f);
        // if (Speed > 5f && Speed < 7) AddReward(1f);
        // if (Speed < 1f) AddReward(-1f);
        // if (Speed < 1f) speedTimer += Time.fixedDeltaTime;
        // else speedTimer = 0;
        //
        // if (speedTimer > 4)
        // {
        //     AddReward(-0.04f);
        //     //finished = true;
        // }

        CheckIfCloseToWall();
        
        if (finished)
        { 
            FinishEpisode();
        }


    }

    private void CheckIfCloseToWall()
    {
        var rayInput = sideRays.GetRayPerceptionInput();
        var rayOutput = RayPerceptionSensor.Perceive(rayInput);

        if (Speed < 1f)
        {
            AddReward(-0.04f);
            return;
        }
        
        for (int i = 3; i < rayOutput.RayOutputs.Length; i++)
        {
            if (rayOutput.RayOutputs[i].HitFraction > 0.08f )
            {
                AddReward(+0.2f);
            }
        }
        
        
    }
}
