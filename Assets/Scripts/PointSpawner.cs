using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSpawner : MonoBehaviour
{
    //The rate at which points are spawned
    public float SpawnInterval;

    //The last point in time at which a point was spawned
    private float LastSpawn;

    //Reference point to clone
    public Point PrefabParticle;

    //Points need a convex bounds to be simulated within or else they will be ignored
    public ConvexBounds SpawnPoint;

    //The main simulation list of points needs to be pinged with a newly spawned point
    public MainTest Simulator;

    //Upper limit on particles spawnable by this
    public int MaxParticles;

    public Material temp;

    private int CurrentParticles = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time-LastSpawn>=SpawnInterval && CurrentParticles<MaxParticles)
        {
            Point Temp = Instantiate(PrefabParticle, transform.position, transform.rotation);

            Temp.Bounds = SpawnPoint;
            Temp.RandomiseVelocity = true;
            Temp.gameObject.GetComponent<MeshRenderer>().material = temp;
            Simulator.PingPoint(Temp);


            CurrentParticles++;
            LastSpawn = Time.time;
        }


    }
}
