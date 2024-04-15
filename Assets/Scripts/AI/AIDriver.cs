using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Serialization;

public class AIDriver : Agent
{
    [SerializeField] private Transform startPos; 
    [SerializeField] private CarController carController;
    private Rigidbody carRB;
    private int checkpointsReached = 0;
    private float timeStill = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        carController = GetComponent<CarController>();
        carRB = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        carController.ResetMovement();
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;
        checkpointsReached = 0;
    }
    
    

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(carRB.velocity);
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
        if (other.TryGetComponent<Checkpoint>(out _))
        {
            AddReward(+1);
            checkpointsReached++;
        }

        if (checkpointsReached >= 24)
        {
            AddReward(+2);
            EndEpisode();
            Debug.Log("Reward: " + GetCumulativeReward());
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent<Wall>(out _))
        {
            AddReward(-1);
            EndEpisode();
            Debug.Log("Reward: " + GetCumulativeReward());
        }
    }

    private void FixedUpdate()
    {
        Debug.Log(Time.timeScale);
        if (carRB.velocity.magnitude <= 0.1f)
        {
            timeStill+= Time.fixedDeltaTime;
            
            if (timeStill >= 5)
            {
                timeStill = 0;
                AddReward(-2f);
                EndEpisode();
                Debug.Log("Reward: " + GetCumulativeReward());
            }
        }
        else timeStill = 0;
    }
}
