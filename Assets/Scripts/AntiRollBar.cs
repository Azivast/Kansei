using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    public Suspension LeftSuspension, RightSuspension;
    public float RollBarStiffness;


    private void FixedUpdate()
    {
        float antiRollForce = LeftSuspension;
        
    }
}
