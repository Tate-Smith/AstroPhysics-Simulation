using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer), typeof(Rigidbody))]
public class TrajectoryPredictor : MonoBehaviour
{
    public int numSteps = 1000; // How many points in the trajectory
    public float timeStep = 0.1f; // Time between each step in the simulation
    public float simulationScale = 1f; // You can tweak this if gravity is too weak/strong

    private LineRenderer lineRenderer;
    private Rigidbody rb;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();

        lineRenderer.positionCount = numSteps;
        lineRenderer.widthMultiplier = 0.1f;

        InvokeRepeating(nameof(UpdateTrajectory), 0f, 0.5f); // Update every 0.5s
    }

    void UpdateTrajectory()
    {
        Vector3[] trajectory = new Vector3[numSteps];

        Vector3 simulatedPosition = transform.position;
        Vector3 simulatedVelocity = rb.linearVelocity;

        for (int i = 0; i < numSteps; i++)
        {
            trajectory[i] = simulatedPosition;

            // Apply gravity from other bodies
            Vector3 acceleration = Vector3.zero;
            foreach (var body in CelestialBody.bodies)
            {
                if (body.gameObject == this.gameObject) continue;

                Vector3 dir = body.transform.position - simulatedPosition;
                float distSqr = dir.sqrMagnitude;
                if (distSqr == 0f) continue;

                float force = (6.674f * rb.mass * body.rb.mass) / distSqr;
                acceleration += dir.normalized * force / rb.mass;
            }

            // Euler integration
            simulatedVelocity += acceleration * timeStep * simulationScale;
            simulatedPosition += simulatedVelocity * timeStep * simulationScale;
        }

        lineRenderer.SetPositions(trajectory);
    }
}
