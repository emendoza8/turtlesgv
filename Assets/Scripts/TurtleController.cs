//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class TurtleAnimControll : MonoBehaviour
{
    public Animator myAnim;
    public float moveSpeed = 5f;

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
        else if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= transform.right;
            myAnim.SetBool("Walk", true); // Set the "Walk" parameter in the Animator to true
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right;
            myAnim.SetBool("Walk", true); // Set the "Walk" parameter in the Animator to true
        }
        else
        {
            // No movement input, stop walking animation
            myAnim.SetBool("Walk", false); // Set the "Walk" parameter in the Animator to false
        }

            moveDirection.Normalize();

        // Move the turtle based on the calculated movement direction
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
}
