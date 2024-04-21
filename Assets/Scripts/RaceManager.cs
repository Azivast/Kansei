using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private CarController[] drivers;
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject LooseScreen;
    [SerializeField] private GameObject Countdown;
    private TMP_Text countdownText;
    private bool raceActive;

    private void Awake()
    {
        countdownText = Countdown.GetComponent<TMP_Text>();
    }

    void Start()
    {
        StartCoroutine(BeginRaceCountdown());
    }

    private IEnumerator BeginRaceCountdown()
    {
        foreach (var driver in drivers)
        {
            driver.ClutchDepressed = true;
        }
        Countdown.SetActive(true);
        
        for (int i = 3; i >= 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        
        foreach (var driver in drivers)
        {
            driver.ClutchDepressed = false;
        }
        Countdown.SetActive(false);
        StartRace();
    }
    
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
        WinScreen.SetActive(true);
    }

    private void OnLoose()
    {
        foreach (var driver in drivers)
        {
            driver.enabled = false;
        }
        LooseScreen.SetActive(true);
    }
}
