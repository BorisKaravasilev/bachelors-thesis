﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float speed = 2f;

	private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
	    rb = GetComponent<Rigidbody>();
	    rb.useGravity = false;
	    rb.freezeRotation = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	    PlayerMovement();

	    if (rb.velocity.magnitude > 0.1f)
	    {
		    transform.forward = rb.velocity;
		}

	    rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
    }

    void PlayerMovement()
    {
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			rb.AddForce(Vector3.left * speed);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			rb.AddForce(Vector3.right * speed);
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			rb.AddForce(Vector3.forward * speed);
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			rb.AddForce(Vector3.back * speed);
		}
	}
}
