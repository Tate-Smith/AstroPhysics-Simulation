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

    public static int time = 10;
    public float scale = 1f;
    public bodyType type;
    public float G = 6.674f;
    public Rigidbody rb;
    public Renderer renderer;
    public static List<CelestialBody> bodies;
    public float speed = 10f;
    public int mass;
    private float radius;
    public Transform body;

    void Start()
    {   
        rb.linearVelocity = transform.forward * speed;
    }

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
    }

    void OnDisable() {
        // removes the body from the list when deleted
        bodies.Remove(this);
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

        rbToAttract.AddForce(force );
    }
}
