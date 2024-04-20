using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CheckpointCollection : MonoBehaviour
{
    private List<Checkpoint> checkpoints;
    [SerializeField] private bool reversed;

    private void Awake()
    {
        checkpoints = new List<Checkpoint>();
        foreach (Transform child in transform)
        {
            Checkpoint checkpoint = child.GetComponent<Checkpoint>();
            if (checkpoint != null)
            {
                checkpoints.Add(checkpoint);
            }
        }
        if (reversed) checkpoints.Reverse();
    }
    
    public Checkpoint GetFirstCheckpoint()
    {
        if (checkpoints.Count > 0)
        {
            return checkpoints[0];
        }
        else return null;
    }
    
    public Checkpoint GetNextCheckpoint(Checkpoint currentCheckpoint)
    {
        int index = checkpoints.IndexOf(currentCheckpoint);
        if (index < checkpoints.Count - 1)
        {
            return checkpoints[index + 1];
        }
        else return null;
    }
}
