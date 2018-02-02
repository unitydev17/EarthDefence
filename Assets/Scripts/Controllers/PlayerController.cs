using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
	const string ENEMY_BULLET_TAG = "EnemyBullet";

	public const float BULLET_SPAWN_DISTANCE = 0.5f;
	private const float BULLET_SPEED = 100f;
	private const float SHIP_SPEED = 10f;
	private const float SHIP_STOPPED_SPEED = 1f;
	private const int MOUSE_AMPLIFIER = 3;

	public GameObject bulletPrefab;
	private float rotationY;


	private void Update()
	{
		HandleInput();
		HandleMouseDirections();
	}


	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(0)) {
			Fire();
		}

		// Forward
		if (Input.GetKey(KeyCode.W)) {
			GetComponent<Rigidbody>().AddForce(transform.forward * SHIP_SPEED);
		}

		// Brake
		if (Input.GetKey(KeyCode.S)) {
			Brake();
		}

		// Right incline
		if (Input.GetKey(KeyCode.D)) {
		}

		// Left incline
		if (Input.GetKey(KeyCode.A)) {
		}		
	}


	void Brake()
	{
		Rigidbody rigidBody = GetComponent<Rigidbody>();
		if (Mathf.Abs(rigidBody.velocity.magnitude) > SHIP_STOPPED_SPEED) {
			rigidBody.AddForce(-transform.forward * SHIP_SPEED);
			return;
		}
		rigidBody.velocity = Vector3.zero;
	}


	void Fire()
	{
		Vector3 position = transform.position + transform.forward * BULLET_SPAWN_DISTANCE;
		GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
		bullet.GetComponent<Rigidbody>().velocity = transform.forward * BULLET_SPEED;
	}


	private void HandleMouseDirections()
	{
		float dx = Input.GetAxis("Mouse X") * MOUSE_AMPLIFIER;
		float dy = Input.GetAxis("Mouse Y") * MOUSE_AMPLIFIER;
		transform.RotateAround(transform.position, Vector3.up, dx);
		transform.RotateAround(transform.position, transform.right, -dy);
	}


	private void OnTriggerEnter(Collider other)
	{
		if (ENEMY_BULLET_TAG == other.gameObject.tag) {
			Destroy(other.gameObject);
		}
	}
}
