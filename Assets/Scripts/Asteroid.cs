using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Asteroid : MonoBehaviour {
	public int minSpeed = -10;
	public int maxSpeed = 10;
	public int rotateSpeed = 20;

	private Rigidbody rb3;
	private Vector3 astDt;

	// Use this for initialization
	void Start () {
		rb3 = GetComponent<Rigidbody> ();
		astDt = RandomDt ();
	}
	Vector3 RandomDt() {
		float deltaX = Random.Range(minSpeed,maxSpeed);
		float deltaY = Random.Range(minSpeed,maxSpeed);
		float deltaZ = Random.Range(minSpeed,maxSpeed); 
		Vector3 asterDt = new Vector3 (deltaX, deltaY, deltaZ);
		return asterDt;
	}	
	// Update is called once per frame
	void Update () {
		var vel = rb3.velocity;
		vel.x = astDt.x;
		vel.y = astDt.y;
		vel.z = astDt.z;
		rb3.velocity = vel;
		transform.Rotate (rotateSpeed*astDt.x*Time.deltaTime,
			rotateSpeed*astDt.y*Time.deltaTime, 
			rotateSpeed*astDt.z*Time.deltaTime);
	}
}
