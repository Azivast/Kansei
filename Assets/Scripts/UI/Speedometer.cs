using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private WheelCollider measuredWheel;
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private string suffix = " km/h";
    private TMP_Text text; 

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
    private void Update()
    {
        text.text = (int)(carRB.velocity.magnitude * 3.6) + suffix;
    }
}
