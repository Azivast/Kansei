using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AIDriver : Agent
{
    [SerializeField] private Transform startPos; 
    [SerializeField] private CarController carController;
    [SerializeField] private CheckpointCollection checkpoints;
    private Checkpoint targetCheckpoint;
    private Rigidbody carRB;
    private float timer = 0;
    private bool goalReached = false;
    private double time;

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

        time = Time.time;
    }
    
    private void FinishEpisode()
    {
        float timeTaken = (float)(Time.time - time);
        AddReward(-timeTaken);
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(carRB.velocity);
        sensor.AddObservation(carRB.angularVelocity);
        sensor.AddObservation(carRB.transform.forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float steer = actions.ContinuousActions[0];
        // float throttle = (actions.ContinuousActions[1]+1)*0.5f; // make range 0-1
        // float brake = (actions.ContinuousActions[2]+1)*0.5f; // make range 0-1
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
            AddReward(+10);
            FinishEpisode();
            return;
        }
        
        if (other.TryGetComponent<Checkpoint>(out Checkpoint thisCheckpoint))
        {
            if (thisCheckpoint == targetCheckpoint)
            {
                targetCheckpoint = checkpoints.GetNextCheckpoint(targetCheckpoint);
                AddReward(+2);
            }
            else
            {
                AddReward(-2);
                Debug.Log("Car drove wrong way");
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent<Wall>(out _))
        {
            AddReward(-10f);
            //FinishEpisode();
        }
    }

    private void FixedUpdate()
    {
        // if (carRB.velocity.magnitude < 0.1f)
        // {
        //     timer += Time.fixedDeltaTime;
        //     if (timer > 1f)
        //     {
        //         //AddReward(-1f);
        //         FinishEpisode();
        //     }
        // }
        // else
        // {
        //     timer = 0;
        // }
    }
}
