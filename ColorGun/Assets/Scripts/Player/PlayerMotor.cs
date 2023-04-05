using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 rotateCamera = Vector3.zero;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    //Get a move vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    //Get a Rotate vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }
    //Get a Rotate Camera vector
    public void RotateCamera(Vector3 _rotationCamera)
    {
        rotateCamera = _rotationCamera;
    }
    //Run every physics iteration
    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }
    //Perform move base on velocity variable
    void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }    
    }  
    
    //Perform rotation
    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if(cam != null)
        {
            cam.transform.Rotate(-rotateCamera);
        }
    }
}
