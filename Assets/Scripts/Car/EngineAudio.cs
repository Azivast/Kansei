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

    private void Update() // todo: lots of magic numbers here that could be explained
    {
        speedRatio = Mathf.Abs(controller.SpeedRatio);
        float speedDirection = Mathf.Sign(controller.Speed);


        // if (speedRatio > limiterThreshold)
        // {
        //     revLimiter = (Mathf.Sin(Time.time*limiterFrequency)+1f)*limiterPitch*(speedRatio-limiterThreshold);
        // }
        // idleSound.volume = Mathf.Lerp(idleMaxVolume, 0.01f, speedRatio);
        // if (speedDirection > 0) // driving forwards
        // {
        //     reverseSound.volume = 0;
        //     revvingSound.volume = Mathf.Lerp(0.3f, revvingMaxVolume, speedRatio);
        //     revvingSound.pitch = Mathf.Lerp(revvingSound.pitch,
        //         Mathf.Lerp(0.3f, revvingMaxPitch, speedRatio) + revLimiter,
        //         Time.deltaTime * 5); // fix outer lerp, delays sound and sounds like shit
        // }
        // else // driving backwards
        // {
        //     //todo: no reverse gear atm so this is untested!
        //     revvingSound.volume = 0;
        //     reverseSound.volume = Mathf.Lerp(0f, reverseMaxVolume, speedRatio);
        //     reverseSound.pitch = Mathf.Lerp(reverseSound.pitch,
        //         Mathf.Lerp(0.2f, reverseMaxPitch, speedRatio) + revLimiter,
        //         Time.deltaTime * 5); // fix outer lerp, delays sound and sounds like shit
        // }
        
        if (speedRatio > limiterThreshold)
        {
            revLimiter = (Mathf.Sin(Time.time*limiterFrequency)+1f)*limiterPitch*(speedRatio-limiterThreshold);
        }
        revvingSound.volume = Mathf.Lerp(idleVolume, revvingMaxVolume, speedRatio);
        revvingSound.pitch = Mathf.Lerp(revvingSound.pitch,
            Mathf.Lerp(idlePitch, revvingMaxPitch, speedRatio) + revLimiter,
            Time.deltaTime * 5); // fix outer lerp, delays sound and sounds like shit
    }
}
