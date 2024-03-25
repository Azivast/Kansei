using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private Rigidbody carRB;

    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springForce;
    private float springVelocity;
    private float damperForce;

    [Header("Wheel")]
    public float wheelRadius;

    private Vector3 suspensionForce;


    void Start()
    {
        minLength = restLength-springTravel;
        maxLength = restLength+springTravel;
    }

    private void FixedUpdate() {
        Debug.Log(maxLength + wheelRadius);
        if (Physics.Raycast(transform.position, -carRB.transform.up, out RaycastHit hit, 100))
        {
            lastLength = springLength;
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (lastLength-springLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (restLength - springLength);
            damperForce = damperStiffness * springVelocity;

            suspensionForce = (springForce+damperForce)*carRB.transform.up;

            carRB.AddForceAtPosition(suspensionForce, hit.point);
        }
    }
}
