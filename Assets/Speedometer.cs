using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private WheelCollider measuredWheel;
    private TMP_Text text; 

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
    private void Update()
    {
        text.text = (int)((3 / 25f) * Mathf.PI * measuredWheel.radius * measuredWheel.rpm) + "KM/H";
    }
}
