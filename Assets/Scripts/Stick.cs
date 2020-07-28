using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Stick : MonoBehaviour
{
    //A stick is composed of two points, the stick's sole purpose is to maintain a set distance between these two points, or if it is an inequality restraint, to maintain a 
    public Point A;
    public Point B;

    public bool Draw; //Editor Only 

    //This length value can be determined on runtime or manually entered. Whatever the result, the stick will conform to this length accordingly
    public float Length;

    //Simple determinent for Stick behaviour
    public bool InequalityConstraint = false;

    //Used by inequality constraints for the distance at which the stick is triggered
    public float MinimumDistance = 0;
 
    //Toggle in the editor if the actual distance between points is desired
    public bool DetermineLength;


    private void Awake()
    {
        if(DetermineLength)
            GetLength();
    }



    public void GetLength()
    {
        Length = Vector3.Distance(A.transform.position, B.transform.position);
    }

    public Stick(Point a, Point b, float leng, bool determine = false)
    {
        A = a;
        B = b;
        if (determine)
        {
            GetLength();
            DetermineLength = determine;
        }
        else
        {
            Length = leng;
        }
    }

    
    




   
}
