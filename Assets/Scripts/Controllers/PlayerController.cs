using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : CommonShipController
{

	private enum GunState
	{
		Idle,
		Fire
	}

	public const float BULLET_SPAWN_DISTANCE = 0.5f;
	private const float SHIP_SPEED = 10f;
	private const float SHIP_STOPPED_SPEED = 1f;
	private const int MOUSE_AMPLIFIER = 3;
	private const int CHAIN_FIRE_NUMBERS = 3;

	private Vector3 rightGunPosition = new Vector3(1.967f, 0.276f, 2f);
	private Vector3 leftGunPosition = new Vector3(-1.967f, 0.276f, 2f);
	private float rotationY;

	private float startChainTime;
	private int chainFireNumber;
	private GunState gunState = GunState.Idle;


	private void Update()
	{
		HandleInput();
		HandleMouseDirections();
		ProcessActions();
	}


	private void ProcessActions() {
		if (GunState.Fire == gunState) {
				
			if (Time.time - startChainTime > 0.05f) {

				if (++chainFireNumber == CHAIN_FIRE_NUMBERS) {
					gunState = GunState.Idle;
					return;
				}

				startChainTime = Time.time;
				Fire(leftGunPosition, rightGunPosition);
			}
		}
	}


	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(0)) {
			gunState = GunState.Fire;
			chainFireNumber = 0;
		}

		// Forward
		if (Input.GetKey(KeyCode.W)) {
			rigidBody.AddForce(transform.forward * SHIP_SPEED);
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
		if (Mathf.Abs(rigidBody.velocity.magnitude) > SHIP_STOPPED_SPEED) {
			rigidBody.AddForce(-rigidBody.velocity.normalized * SHIP_SPEED);
			return;
		}
		rigidBody.velocity = Vector3.zero;
	}




	private void HandleMouseDirections()
	{
		float dx = Input.GetAxis("Mouse X") * MOUSE_AMPLIFIER;
		float dy = Input.GetAxis("Mouse Y") * MOUSE_AMPLIFIER;
		transform.RotateAround(transform.position, Vector3.up, dx);
		transform.RotateAround(transform.position, transform.right, -dy);
	}

}
