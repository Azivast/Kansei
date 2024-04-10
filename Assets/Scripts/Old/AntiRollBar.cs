using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AntiRollBar : MonoBehaviour
{
    [FormerlySerializedAs("LeftSuspension")] public SuspensionPhysics leftSuspensionPhysics;
    [FormerlySerializedAs("RightSuspension")] public SuspensionPhysics rightSuspensionPhysics;
    public float RollBarStiffness;


    private void FixedUpdate()
    {
        //float antiRollForce = LeftSuspension;
        
    }
}
