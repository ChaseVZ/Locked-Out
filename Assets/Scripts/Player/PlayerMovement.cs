using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Custom UI line
// public KeyCode jump {get; set;}
// jump = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jumpkey", "Space"));

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float sprintSpeed = 2f;
    public float gravity = -9.81f;
    public float gravityMagnitude = 2f;
    public float jumpHeight = 3f;

    public Text debug;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool sprinting;

    void Start()
    {
        sprinting = false;
    }

    void Update()
    {
        // CHECK if player is sprinting 
        if (Input.GetButtonDown("Sprint"))
        {
            sprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            sprinting = false;
        }

        // CHECK if player is touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // UPDATE player pos based on movement inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (sprinting)
            controller.Move(move * speed * sprintSpeed * Time.deltaTime);
        else
            controller.Move(move * speed * Time.deltaTime);

        // CHECK if player is trying to jump and update pos&velocity
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * gravityMagnitude * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (debug)
        {
            if (isGrounded)
                debug.text = "Grounded: " + velocity.y;
            else
                debug.text = "Not Grounded: " + velocity.y;
        }
    }
}
