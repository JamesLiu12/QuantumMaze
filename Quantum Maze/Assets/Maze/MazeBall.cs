using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBall : MonoBehaviour
{
    public Vector3 Destination;
    public bool Moving = false;
    public float Speed = 1.0f;
    public float Epsilon = 0.05f;

    public MazeBall(Vector3 destination)
    {
        Destination = destination;
    }

    void FixedUpdate()
    {
        if (!Moving) return;

        Vector3 positionDifference = Destination - transform.position;
        if (positionDifference.magnitude <= Epsilon) return;

        Vector3 velocity = positionDifference * Speed;
        transform.position += velocity * Time.deltaTime;
    }

    public void Move() { Moving = true; }

    public void Stop() { Moving = false; }
}
