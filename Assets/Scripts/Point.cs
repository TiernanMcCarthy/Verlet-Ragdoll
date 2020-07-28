using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    //Previous velocity is stored as the current transform every frame after a new position is calculated
    public Vector3 PreviousPosition;

    //Particles that are pinned are ignored and act as if they're kinematic
    public bool Pinned = false;

    //Modifies how much velocity is retained after a collision
    public float Bounciness = 0.5f;

    //Toggle wether velocity should be lost over time
    public bool ApplyFriction = true;

    //All simulation of points is made within convex bounds. Each Particle needs one assigned to it in order to be simulated
    public ConvexBounds Bounds;

    //Toggle a new velocity change
    public bool RandomiseVelocity;


    public float SpeedFactor;


    private void Start()
    {
        PreviousPosition = transform.position;
        ApplyFriction = true;

    }
    private void FixedUpdate()
    {

        if(RandomiseVelocity==true)
        {
            PreviousPosition -= new Vector3(Random.Range(-Bounds.Width, Bounds.Width) / SpeedFactor, Random.Range(-Bounds.Height, Bounds.Height) / SpeedFactor, Random.Range(-Bounds.Length, Bounds.Length) / SpeedFactor);
            RandomiseVelocity = false;
        }

        
    }


}
