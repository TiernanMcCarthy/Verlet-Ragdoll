using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;


//Deprecated implementation using jobs
public struct UpdatePoints : IJobParallelFor
{

    public NativeArray<float3> Position;
    public NativeArray<float3> PreviousPosition;
    public float Friction;

    public float timeStep;

    public float gravity;

    public NativeArray<bool> Pinned;


    public void Execute(int index)
    {

        if (Pinned[index] != true)
        {
            float vx = (Position[index].x - PreviousPosition[index].x) * Friction;
            float vy = (Position[index].y - PreviousPosition[index].y) * Friction;
            float vz = (Position[index].z - PreviousPosition[index].z) * Friction;

            PreviousPosition[index] = Position[index];
            Position[index] = Position[index] + new float3(vx, vy, vz) * timeStep;
            Position[index] -= new float3(0, gravity * timeStep, 0);
        }


    }
}

public class MainTest : MonoBehaviour
{

    //Key Simulation variables
    public float Gravity; //Gravitational force in the Y axis.

    public float Bounce; //This determines how much force a particle can lose upon impact with bounds, e.g. 0.6 implies 60% of velocity is retained in the inverse of the collision.

    public float friction = 0.99f; //General friction applied at all times, this ensures that objects will naturally come to a rest over time


    public float TimeStep;



    //This prevents warping of a compound stick object by allowing the sticks to reach a near equilibrium
    public int ConstraintIterations = 3;





    List<Point> Points;  //Points (particles) are the main object that is simulated, this list contains all of the main particles

    List<Stick> Sticks; //Sticks ensure constraints between points are met, these are iterated through constantly and ensure stability



    // Start is called before the first frame update
    void Start()
    {
        Stick[] Stick2; //These lists are temporarily used to collect the respective objects and get added to the correct list datatype
        Point[] Point2;

        //Conversion to this list format is simply open in case of expansion, where points could easily be added or removed without issue
        Points = new List<Point>();
        Point2 = FindObjectsOfType<Point>();

        for (int i = 0; i < Point2.Length; i++)
        {
            Points.Add(Point2[i]);
        }

        Sticks = new List<Stick>();
        Stick2 = FindObjectsOfType<Stick>();
        for (int i = 0; i < Stick2.Length; i++)
        {
            Sticks.Add(Stick2[i]);
        }



    }

    //Adds a new point into the point list for spawning new points as the simulation runs
    public void PingPoint(Point p) 
    {
        Points.Add(p);
    }


    /// <summary>
    /// Called by the UI button for randomising particle velocity
    /// </summary>
    public void RandomiseVelocity()
    {
        for (int i = 0; i < Points.Count; i++)
        {
            Points[i].RandomiseVelocity = true;
        }
    }




    /// <summary>
    /// Ensures points stay within their bounding zone, this needs to be ran every time a stick or point is moved in order to preserve stability
    /// </summary>
    void ConstrainPoints()
    {

        for (int i = 0; i < Points.Count; i++) 
        {

            Point p = Points[i];
            if (!p.Pinned) //Points can be pinned in place and stop responding to physical stimulus, this is accounted for
            {
                float vx = (p.transform.position.x - p.PreviousPosition.x) * friction; //Velocity in Verlet is explicitly solved by previous and current positions, the end "velocity" for this frame is dampened by the friction variable
                float vy = (p.transform.position.y - p.PreviousPosition.y) * friction;
                float vz = (p.transform.position.z - p.PreviousPosition.z) * friction;


                //Simple bounds testing takes place for all 3 axes. The current position need only be moved to the edge of the bounds if an offence is found, the velocity of the "impact" is reflected on the return
                if (p.transform.position.x > p.Bounds.transform.position.x + p.Bounds.Width)
                {
                    p.transform.position = new Vector3(p.Bounds.transform.position.x + p.Bounds.Width, p.transform.position.y, p.transform.position.z);
                    p.PreviousPosition.x = p.transform.position.x + vx * p.Bounciness;
                }
                else if (p.transform.position.x < p.Bounds.transform.position.x - p.Bounds.Width)
                {
                    p.transform.position = new Vector3(p.Bounds.transform.position.x - p.Bounds.Width, p.transform.position.y, p.transform.position.z);
                    p.PreviousPosition.x = p.transform.position.x + vx * p.Bounciness;
                }
                if (p.transform.position.y > p.Bounds.transform.position.y + p.Bounds.Height)
                {
                    p.transform.position = new Vector3(p.transform.position.x, p.Bounds.transform.position.y + p.Bounds.Height, p.transform.position.z);
                    p.PreviousPosition.y = p.transform.position.y + vy * p.Bounciness;
                }
                else if (p.transform.position.y < p.Bounds.transform.position.y - p.Bounds.Height)
                {
                    p.transform.position = new Vector3(p.transform.position.x, p.Bounds.transform.position.y - p.Bounds.Height, p.transform.position.z);
                    p.PreviousPosition.y = p.transform.position.y + vy * p.Bounciness;
                }
                if (p.transform.position.z > p.Bounds.transform.position.z + p.Bounds.Length)
                {
                    p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, p.Bounds.transform.position.z + p.Bounds.Length);
                    p.PreviousPosition.z = p.transform.position.z + vz * p.Bounciness;
                }
                else if (p.transform.position.z < p.Bounds.transform.position.z - p.Bounds.Length)
                {
                    p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, p.Bounds.transform.position.z - p.Bounds.Length);
                    p.PreviousPosition.z = p.transform.position.z + vz * p.Bounciness;
                }



            }
        }
    }


    /// <summary>
    /// Points (Particles) are calculated here, in accordance to normal verlet principles
    /// </summary>
    void UpdatePoints()
    {

        for (int i = 0; i < Points.Count; i++)
        {
            Point p = Points[i];
            if (!p.Pinned)
            {

                float vx; //Velocity variables for modifying the position
                float vy;
                float vz;

                if (p.ApplyFriction) //Some objects can be frictionless as a result of this check
                {
                    //Velocity can be solved by calculating the distance moved between the current position and previous position due to the fixed timestep
                    vx = (p.transform.position.x - p.PreviousPosition.x) * friction;
                    vy = (p.transform.position.y - p.PreviousPosition.y) * friction;
                    vz = (p.transform.position.z - p.PreviousPosition.z) * friction;
                }
                else
                {
                    //Identical implementation as above but without friction
                    vx = (p.transform.position.x - p.PreviousPosition.x);
                    vy = (p.transform.position.y - p.PreviousPosition.y);
                    vz = (p.transform.position.z - p.PreviousPosition.z);
                }

                p.PreviousPosition = p.transform.position; //The previous position needs to be updated to the current position before this velocity is applied

                p.transform.position += new Vector3(vx, vy, vz); //The current position is then updated to reflect this velocity change. 
                p.transform.position = p.transform.position - new Vector3(0, Gravity * TimeStep, 0);//Gravity can now be applied to the end result, the fixed timestep is still required
            }

        }


    }
    private void OnDrawGizmos()
    {

        if (Sticks != null)
        {
            for (int i = 0; i < Sticks.Count; i++)
            {
                if (Sticks[i].Draw)
                {
                    Gizmos.DrawLine(Sticks[i].A.transform.position, Sticks[i].B.transform.position);
                }
            }
        }
    }
    /// <summary>
    /// Sticks are updated and a set distance is maintained
    /// </summary>
    /// <param name="S"></param>
    void StickLoop(Stick S)
    {
        float dx = S.B.transform.position.x - S.A.transform.position.x;
        float dy = S.B.transform.position.y - S.A.transform.position.y;
        float dz = S.B.transform.position.z - S.A.transform.position.z;
        //Get total Distance as a single unit number
        float Distance = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        //Calculate the difference from the desired unit length and get a percentage of difference to multiply by
        float difference = S.Length - Distance;
        float percent = 0;
        if (difference != 0 && Distance != 0)
        {
            percent = difference / Distance / 2;
        }
        //Apply these changes to an offset for the axes.

        float offsetX = dx * percent;
        float offsetY = dy * percent;
        float offsetZ = dz * percent;
        //Apply this to both sides of the stick, in opposite directions and the joint will be within the constraints
        if (!S.A.Pinned)
        {
            S.A.transform.position = S.A.transform.position - new Vector3(offsetX, offsetY, offsetZ);
        }
        if (!S.B.Pinned)
        {
            S.B.transform.position = S.B.transform.position + new Vector3(offsetX, offsetY, offsetZ);
        }

    }
    /// <summary>
    /// Inequality constraints are calculated here, they will only trigger if the minimum distance value is met
    /// </summary>
    /// <param name="S"></param>
    /// <param name="Minimum"></param>
    void StickLoop(Stick S, float Minimum)
    {
        float dx = S.B.transform.position.x - S.A.transform.position.x;
        float dy = S.B.transform.position.y - S.A.transform.position.y;
        float dz = S.B.transform.position.z - S.A.transform.position.z;
        //Get total Distance as a single unit number
        float Distance = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        //Calculate the difference from the desired unit length and get a percentage of difference to multiply by
        float difference = Minimum - Distance;
        float percent = 0;
        if (difference != 0 && Distance != 0)
        {
            percent = difference / Distance / 2;
        }
        //Apply these changes to an offset for the axes.

        float offsetX = dx * percent;
        float offsetY = dy * percent;
        float offsetZ = dz * percent;
        //Apply this to both sides of the stick, in opposite directions and the joint will be within the constraints
        if (!S.A.Pinned)
        {
            S.A.transform.position = S.A.transform.position - new Vector3(offsetX, offsetY, offsetZ);
        }
        if (!S.B.Pinned)
        {
            S.B.transform.position = S.B.transform.position + new Vector3(offsetX, offsetY, offsetZ);
        }

    }



    void UpdateSticks()
    {
        for (int i = 0; i < Sticks.Count; i++)
        {
            Stick S = Sticks[i];
            //Distance on All 3 Axes

            if (S.InequalityConstraint)
            {
                if (Vector3.Distance(S.A.transform.position, S.B.transform.position) < S.MinimumDistance)
                {
                    StickLoop(S, S.MinimumDistance);
                }
            }
            else
            {
                StickLoop(S);
            }


        }
    }

 

    // Update is called once per frame
    private void Update()
    {
        //A timestep is required for these particles to be calculated
        TimeStep = Time.fixedDeltaTime * Time.fixedDeltaTime;


        //The basic structure of the verlet loop is here. Points are updated, and then below sticks and points are updated to meet within simulation rules
        UpdatePoints(); 
        for (int i = 0; i < ConstraintIterations; i++) //These two functions interfere with eachother and warp values, this loop settles them after a few iterations, 3 is a good minimum.
        {
            UpdateSticks();

            ConstrainPoints();
        }

    }

    //Deprecated jobs system
    void Notes()
    {
        //Initialise PositionArrays and Pinned Status for calculation
        NativeArray<float3> PositionArray = new NativeArray<float3>(Points.Count, Allocator.TempJob);
        NativeArray<float3> PreviousPositionArray = new NativeArray<float3>(Points.Count, Allocator.TempJob);
        NativeArray<bool> PinnedArray = new NativeArray<bool>(Points.Count, Allocator.TempJob);
        for (int i = 0; i < Points.Count; i++) //Fill these arrays with information
        {
            PositionArray[i] = Points[i].transform.position;
            PreviousPositionArray[i] = Points[i].PreviousPosition;
            PinnedArray[i] = Points[i].Pinned;
        }
        //Create Job Handler
        UpdatePoints Job = new UpdatePoints { Position = PositionArray, PreviousPosition = PreviousPositionArray, Pinned = PinnedArray, gravity = Gravity, Friction = friction, timeStep = TimeStep };
        //schedule Job
        JobHandle temp = Job.Schedule(PositionArray.Length, 800);
        //Start Job
        temp.Complete();
        //Refill Points Array with new information
        for (int i = 0; i < Points.Count; i++)
        {
            Points[i].transform.position = PositionArray[i];
            Points[i].PreviousPosition = PreviousPositionArray[i];
        }

        PositionArray.Dispose();
        PreviousPositionArray.Dispose();
        PinnedArray.Dispose();


    }


}
