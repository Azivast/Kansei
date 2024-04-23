using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private CarController[] drivers;
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject LooseScreen;
    [SerializeField] private GameObject Countdown;
    private TMP_Text countdownText;
    private bool raceActive;
    private int carsFinished;

    private void Awake()
    {
        countdownText = Countdown.GetComponent<TMP_Text>();
    }

    void Start()
    {
        StartCoroutine(BeginRaceCountdown());
        WinScreen.SetActive(false);
        LooseScreen.SetActive(false);
        LapTime.CurrentTime = 0;
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
    
    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("MainMenu");
    }
    
    void Update()
    {
        if (raceActive) LapTime.CurrentTime += Time.deltaTime;
    }

    private void StartRace()
    {
        LapTime.CurrentTime = 0;
        carsFinished = 0;
        raceActive = true;
    }

    public void GoalReached(CarController car)
    {
        carsFinished++;
        car.CompleteStop = true;
        if (car.AIControlled) return;


        if (carsFinished == 1)
        {
            WinScreen.SetActive(true);
            raceActive = false;
            StartCoroutine(ReturnToMainMenu());
        }
        else
        {
            LooseScreen.SetActive(true);
            raceActive = false;
            StartCoroutine(ReturnToMainMenu());
        }
    }
}
