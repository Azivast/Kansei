using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LapTimeUI : MonoBehaviour
{

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.text = "Lap Time: " + LapTime.CurrentTime.ToString("F1") + " seconds";
    }
}
