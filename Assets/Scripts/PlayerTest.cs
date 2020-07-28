using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{

    public float speed;
    public float sensitivity;


    float rotationamount;
    public GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Vertical") != 0)
        {

            transform.position += transform.forward * speed * Input.GetAxisRaw("Vertical") * Time.deltaTime;
        }

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            //Vector3.Cross(transform.forward * Input.GetAxis("Horizontal") * speed, transform.up * -1);
            Vector3 Temp = Vector3.Cross(transform.forward * Input.GetAxis("Horizontal") * speed, transform.up * -1);
            transform.position += Temp * Time.deltaTime;
        }

        if (Input.GetAxisRaw("Mouse X") != 0)
        {
            Vector3 Euler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(Euler.x, Euler.y + Input.GetAxisRaw("Mouse X") * sensitivity, Euler.z);

        }

        if (Input.GetAxisRaw("Mouse Y") != 0)
        {
            Vector3 Euler = Camera.transform.rotation.eulerAngles;
            if (rotationamount + sensitivity * Input.GetAxisRaw("Mouse Y") > 90)
            {
                rotationamount = 90;
            }
            else if (rotationamount + sensitivity * Input.GetAxisRaw("Mouse Y") < -90)
            {
                rotationamount = -90;
            }
            else
            {
                rotationamount += sensitivity * Input.GetAxisRaw("Mouse Y");
            }

            Camera.transform.rotation = Quaternion.Euler(-rotationamount, Euler.y, Euler.z);
        }
    }
}