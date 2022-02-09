using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    private float speed = 5.2f;
    public bool onGround = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") > 0.1)
        {
            rb.position += Vector3.right * Time.deltaTime * speed;
        }
        else if(Input.GetAxis("Horizontal") < -0.1) rb.position += Vector3.left * Time.deltaTime * speed;
        if (Input.GetAxis("Vertical") > 0.1)
        {
            rb.position += Vector3.forward * Time.deltaTime * speed;
        }
        else if(Input.GetAxis("Vertical") < -0.1) rb.position += -Vector3.forward * Time.deltaTime * speed;

        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rb.velocity += Vector3.up * 5;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.name == "Plane")
        {
            onGround = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.name == "Plane")
        {
            onGround = true;
        }
    }
}
