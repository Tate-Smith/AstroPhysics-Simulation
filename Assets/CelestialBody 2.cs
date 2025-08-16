using UnityEngine;
using System.Collections.Generic;

public class CelestialBody : MonoBehaviour
{
    public enum bodyType {
        BLACKHOLE,
        STAR,
        PLANET,
        GASGIANT,
        MOON
    }

    public enum Orbit {
        ORBIT,
        CUSTOM
    }

    public static int time = 10;
    public float scale = 1f;
    public bodyType type;
    public Orbit orbit;
    public float G = 6.674f;
    public Rigidbody rb;
    public Renderer renderer;
    public static List<CelestialBody> bodies;
    public float velocity;
    public int mass;
    private float radius;
    public CelestialBody objectToOrbit;

    void FixedUpdate() {
        foreach (CelestialBody b in bodies) {
            if (b != this) {
                Attract(b);
            }
        }
    }

    void OnEnable() {
        Time.timeScale = time;
        Time.fixedDeltaTime = 0.02f / time;

        // adds the body to the list of bodies
        if (bodies == null) {
            bodies = new List<CelestialBody>();
        }
        bodies.Add(this);

        rb.mass = mass;

        if (type == bodyType.BLACKHOLE) {
            renderer.material.color = new Color(0f, 0f, 0f);
            radius = 0.001f * mass * scale;
        }
        else if (type == bodyType.STAR) {
            renderer.material.color = new Color(1f, 235f/255f, 4f/255f);
            radius = Mathf.Pow(mass, 1f / 3f) * scale;
        }
        else if (type == bodyType.PLANET) {
            renderer.material.color = new Color(36f/255f, 1f, 0f);
            radius = Mathf.Pow(mass, 1f / 3f) * scale;
        }
        else if (type == bodyType.GASGIANT) {
            renderer.material.color = new Color(0f, 1f, 1f);
            radius = Mathf.Pow(mass, 1f / 3f) * scale;
        }
        else {
            renderer.material.color = new Color(169f/255f, 169f/255f, 169f/255f);
            radius = Mathf.Pow(mass, 1f / 3f) * scale;
        }

        transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
        gameObject.name = type.ToString() + " (" + mass + ")";

        // Calculate distance
        Vector3 radiusVector = transform.position - objectToOrbit.transform.position;
        float distance = radiusVector.magnitude;

        float circularRelativeVelocityMagnitude = 0f;
        if (orbit == Orbit.ORBIT) {
            // v_relative = √G * (M_central + M_orbiter) / distance
            // For circular orbits, the mass of the orbiting body itself also contributes to the "effective" mass in the formula,
            // especially if it's not negligible.
            circularRelativeVelocityMagnitude = Mathf.Sqrt((G * (objectToOrbit.mass + rb.mass)) / distance);
        }
        else if (orbit == Orbit.CUSTOM) {
            velocity = velocity;
        }

        // Get a perpendicular velocity direction in the orbital plane (e.g. XZ)
        Vector3 orbitalPlaneNormal = Vector3.up; // Assuming XZ plane for simplicity, modify if needed for 3D
        Vector3 perpendicularDirection = Vector3.Cross(orbitalPlaneNormal, radiusVector.normalized);

        rb.linearVelocity = objectToOrbit.rb.linearVelocity + (perpendicularDirection * circularRelativeVelocityMagnitude);
    }

    void OnDisable() {
        // removes the body from the list when deleted
        bodies.Remove(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        // when it collides with another body
        Debug.Log("Collided with " + collision.gameObject.name);

        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();

        // the smaller object gets destroyed
        if (this.mass < otherRb.mass) {
            Destroy(gameObject);
        }
    }

    void Attract(CelestialBody ObjToAttract) {
        // function for gravity
        Rigidbody rbToAttract = ObjToAttract.rb;

        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;

        if (distance == 0f) {
            return;
        }

        float forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
        rb.AddForce(-force);
    }
}
