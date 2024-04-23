using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tachometer : MonoBehaviour
{
    private Image fillBar;
    [SerializeField] private EngineAudio engineAudio;

    private void Awake()
    {
        fillBar = GetComponent<Image>();
    }

    private void Update()
    {
        fillBar.fillAmount = engineAudio.RPMRatio;
    }
}
