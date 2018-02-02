using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : MonoBehaviour {
	// structure for create menu in unity
	public enum RotationAxes {
		MouseXAndY = 0,
		MouseX = 1,
		MouseY = 2
	}
	public RotationAxes axes  = RotationAxes.MouseXAndY;
	public float sensHor = 5.0f;
	public float sensVer = 5.0f;

	public float minVer = - 80.0f;
	public float maxVer = 10.0f;

	private float rotataionX = 0;

	// Use this for initialization
	void Start () {
		// turn off world physics for mouse
		Rigidbody body = GetComponent<Rigidbody>();
		if (body != null) {
			body.freezeRotation = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (axes == RotationAxes.MouseX) {
			// add rotation angle automatically
			transform.Rotate (0, Input.GetAxis("Mouse X")*sensHor, 0);
		} 
		else if (axes == RotationAxes.MouseY) {
			// set rotation angle
			rotataionX -= Input.GetAxis ("Mouse Y") * sensVer;
			// Debug.Log(Input.GetAxis ("Mouse Y"));
			// fixing value between min max
			rotataionX = Mathf.Clamp (rotataionX, minVer, maxVer);

			// no horizontal rotation fixing Y
			float rotationY = transform.localEulerAngles.y;

			transform.localEulerAngles = new Vector3 (rotataionX, rotationY, 0);
		}   
		else {
			rotataionX -= Input.GetAxis ("Mouse Y") * sensVer;
			rotataionX = Mathf.Clamp (rotataionX, minVer, maxVer);
			float delta = Input.GetAxis("Mouse X") * sensHor;

			// angle now + delta
			float rotationY = transform.localEulerAngles.y + delta;
			transform.localEulerAngles = new Vector3 (rotataionX, rotationY, 0);

		}
	}
}
