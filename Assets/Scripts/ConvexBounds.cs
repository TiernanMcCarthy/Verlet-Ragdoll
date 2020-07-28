using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvexBounds : MonoBehaviour
{

    //Convex Bounds are defined by a width and height from the centre and a Length Parameter
    public float Width;
    public float Height;
    public float Length;


    public bool move; //Simple boolean deciding on if the bounds should move.

    float mover;


    //Start and end points for the bounds to lerp between two points
    public Transform StartMarker;

    public Transform EndMarker;

    public float speed = 1.0f;

    private float StartTime;

    private float JourneyLength;

    private void Awake()
    {
        StartTime = Time.time;

        JourneyLength = Vector3.Distance(transform.position, EndMarker.transform.position);

    }

    private void Update()
    {
        mover += Time.deltaTime;

        mover = mover % 5f;
        if(move==true)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - StartTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / JourneyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(StartMarker.position, EndMarker.position, fractionOfJourney);

            if(Vector3.Distance(transform.position,EndMarker.transform.position)<0.1f)
            {
                Transform Temp = EndMarker; // Swap markers
                EndMarker = StartMarker;
                StartMarker = Temp;

                StartTime = Time.time;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 255, 0, 0.1f);
        Gizmos.DrawCube(transform.position, new Vector3(Width*2, Height*2, Length*2));
    }
}
