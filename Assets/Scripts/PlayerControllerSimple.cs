using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]

public class PlayerControllerSimple : MonoBehaviour {
	public float engineSpeed = 8f;
	public float maxSpeed = 40;
	public float rotateSpeed = 60f;


	public Rigidbody rb3;

	private bool up;
	private bool down;

	private float deltaZ;
	private float deltaX;
	private float deltaY;


	// Use this for initialization
	void Start () {

		rb3 = GetComponent<Rigidbody> ();
		deltaZ = 0;
		deltaX = 0;
		deltaY = 0;
	}

	void runEngine() {
		
		float speed = Input.GetAxis ("Vertical") * engineSpeed;
		speed *= Time.deltaTime;
		// angle for left right
		float angle = transform.localEulerAngles.y * Mathf.PI/180;
		// angle for up down

		float angleUD = transform.localEulerAngles.x * Mathf.PI/180;
		Debug.Log (angleUD);


		deltaX += Mathf.Sin (angle)*speed;
		deltaY += -Mathf.Sin (angleUD)*speed;
		deltaZ += Mathf.Cos (angle)*speed;

		deltaX = Mathf.Min (Mathf.Max (deltaX, -maxSpeed), maxSpeed);
		deltaY = Mathf.Min (Mathf.Max (deltaY, -maxSpeed), maxSpeed);
		deltaZ = Mathf.Min (Mathf.Max (deltaZ, -maxSpeed), maxSpeed);


//		Debug.Log (deltaX);
//		Debug.Log (deltaY);
//		Debug.Log (deltaZ);
	}

	// Update is called once per frame
	void Update () {

		// move forward
		if (Input.GetAxis ("Vertical") > 0) {
			runEngine ();
		}
		// move up down
		if (Input.GetKeyDown(KeyCode.Q)) {
			up = true;
		}
		if (Input.GetKeyUp (KeyCode.Q)) {
			up = false;
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			down = true;
		}
		if (Input.GetKeyUp(KeyCode.E)) {
			down = false;
		}

		if (up){
			transform.Rotate (-rotateSpeed*Time.deltaTime,0, 0);
		}
		if (down){
			transform.Rotate (rotateSpeed*Time.deltaTime,0, 0);
		}
			
		// move player
		var vel = rb3.velocity;
		vel.x = deltaX;
		vel.y = deltaY;
		vel.z = deltaZ;
		rb3.velocity = vel;

		// rotate
		float rotateY = Input.GetAxis ("Horizontal") * rotateSpeed;
		transform.Rotate (0, rotateY*Time.deltaTime, 0);


//		Debug.Log (movement);
	}
}
