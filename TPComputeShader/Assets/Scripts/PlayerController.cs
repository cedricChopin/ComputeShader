using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    private float speed = 5.0f;
    public bool onGround = false;
    private float turnSmoothVelocity;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.3f);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        
        rb.velocity = direction * speed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onGround) rb.velocity += Vector3.up * 5;
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
