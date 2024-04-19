using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private CarController[] drivers;
    private bool raceActive;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (raceActive) LapTime.CurrentTime += Time.deltaTime;
    }

    private void StartRace()
    {
        LapTime.CurrentTime = 0;
        raceActive = true;
    }

    private void OnWin()
    {
        foreach (var driver in drivers)
        {
            driver.enabled = false;
        }
        // Enable Win UI
    }

    private void OnLoose()
    {
        foreach (var driver in drivers)
        {
            driver.enabled = false;
        }
        // Enable Win UI
    }
}
