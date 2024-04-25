//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class TurtleAnimControll : MonoBehaviour
{
    public Animator myAnim;
    public float moveSpeed = 5f;
    public float rotationSpeed = 90f; // Rotation speed in degrees per second

    // Update is called once per frame
    void Update()
    {
        // Calculate movement direction based on input keys
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward;
            myAnim.SetBool("Walk", true); // Set the "Walk" parameter in the Animator to true
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= transform.forward;
            myAnim.SetBool("Walk", true); // Set the "Walk" parameter in the Animator to true
        }

        // Rotate the turtle based on input keys
        float rotationAmount = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            rotationAmount -= rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotationAmount += rotationSpeed * Time.deltaTime;
        }

        // Rotate the turtle
        transform.Rotate(Vector3.up, rotationAmount);

        // If there's any movement input, normalize the moveDirection vector
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
        }
        else
        {
            // No movement input, stop walking animation
            myAnim.SetBool("Walk", false); // Set the "Walk" parameter in the Animator to false
        }

        // Move the turtle based on the calculated movement direction
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
}
