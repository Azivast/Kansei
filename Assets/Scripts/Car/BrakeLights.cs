using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeLights : MonoBehaviour
{
    [SerializeField] private bool delayTurnOff;
    [SerializeField] private float delay = 0.2f;
    private CarController carController;
    private MeshRenderer renderer;
    private bool didBrakeLastFrame;

    private Coroutine turnOffBrakeLights;
    
    private void Awake()
    {
        carController = GetComponentInParent<CarController>();
        renderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (!delayTurnOff)
        {
            renderer.enabled = carController.IsBraking;
            return;
        }
        
        if (carController.IsBraking)
        {
            if (!renderer.enabled)
            {
                renderer.enabled = true;
            }
            if (turnOffBrakeLights != null)
            {
                StopCoroutine(turnOffBrakeLights);
            }
        }
        else
        {
            if (renderer.enabled)
            {
                turnOffBrakeLights = StartCoroutine(TurnOffBrakeLights());
            }
        }
    }

    private IEnumerator TurnOffBrakeLights()
    {
        yield return new WaitForSeconds(delay);
        renderer.enabled = false;
    }
}
