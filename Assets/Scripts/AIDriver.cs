using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AIDriver : Agent
{
    [SerializeField] private Transform testTarget; 
    [SerializeField] private CarController carController;
    private int checkpointsReached = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        carController = GetComponent<CarController>();
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(0, 1, 0);
        transform.rotation = Quaternion.identity;
        checkpointsReached = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(testTarget.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float throttle = actions.ContinuousActions[0];
        float steer = actions.ContinuousActions[1];
        
        carController.SetInputs(steer, throttle);
    }

    // public override void Heuristic(in ActionBuffers actionsOut)
    // {
    //     actionsOut.
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Checkpoint>(out _))
        {
            AddReward(+1);
            checkpointsReached++;
        }
        if (other.TryGetComponent<Wall>(out _))
        {
            AddReward(-1);
            EndEpisode();
        }
        
        if (checkpointsReached==8) EndEpisode();
    }
}
