using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceGoal : MonoBehaviour
{
    private RaceManager raceManager;

    private void Awake()
    {
        raceManager = GetComponentInParent<RaceManager>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CarController car))
        {
            Debug.Log("Goal Reached");
            raceManager.GoalReached(car);
        }
    }
}
