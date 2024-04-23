using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    private CarController controller;
    [SerializeField] private AudioSource revvingSound;
    [SerializeField] private float revvingMaxVolume;
    [SerializeField] private float revvingMaxPitch;
    
    [SerializeField] private AudioSource idleSound;
    [SerializeField] private float idleVolume;
    [SerializeField] private float idlePitch;
    
    [SerializeField] private AudioSource reverseSound;
    [SerializeField] private float reverseMaxVolume;
    [SerializeField] private float reverseMaxPitch;
    
    
    [SerializeField] private float revLimiter;
    [SerializeField] private float limiterPitch = 0.5f;
    [SerializeField] private float limiterFrequency = 50f;
    [SerializeField] private float limiterThreshold = 0.8f;

    private float speedRatio;
    private float pitch;
    public float RPMRatio => pitch/(revvingMaxPitch*1.5f);

    private void Awake()
    {
        controller = GetComponent<CarController>();
    }

    private void Start()
    {
        reverseSound.volume = 0;
        idleSound.volume = 0;
        revvingSound.volume = 0;
    }

    private void Update()
    {
        speedRatio = Mathf.Abs(controller.SpeedRatio);
        float speedDirection = Mathf.Sign(controller.Speed);
        
        if (speedRatio > limiterThreshold)
        {
            revLimiter = (Mathf.Sin(Time.time*limiterFrequency)+1f)*limiterPitch*(speedRatio-limiterThreshold);
        }
        revvingSound.volume = Mathf.Lerp(idleVolume, revvingMaxVolume, speedRatio);
        pitch = Mathf.Lerp(revvingSound.pitch,
            Mathf.Lerp(idlePitch, revvingMaxPitch, speedRatio) + revLimiter,
            Time.deltaTime * 5); // Todo: this outer lerp is a little funky, maybe refactor
        revvingSound.pitch = pitch;
    }
}
