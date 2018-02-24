using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : CommonShipController
{

	private enum GunState
	{
		Idle,
		Fire
	}


	public const float BULLET_SPAWN_DISTANCE = 0.5f;
	private const float SHIP_SPEED = 1f;
	private const float SHIP_STOPPED_SPEED = 1f;
	private const int MOUSE_AMPLIFIER = 3;
	private const int CHAIN_FIRE_NUMBERS = 3;

	private Vector3 rightGunPosition = new Vector3(1.967f, 0.276f, 2f);
	private Vector3 leftGunPosition = new Vector3(-1.967f, 0.276f, 2f);
	private float rotationY;

	private float startChainTime;
	private int chainFireNumber;
	private GunState gunState = GunState.Idle;

	private Quaternion startRotation;
	public static event Action cameraActions;




	void Start() {
		base.Start();
		startRotation = transform.rotation;
	}


	private void Update()
	{
		HandleInput();
		ProcessActions();
	}

	private void LateUpdate() {
		HandleMouseDirections();
	}


	private void ProcessActions()
	{
		if (GunState.Fire == gunState) {
				
			if (Time.time - startChainTime > 0.02f) {

				if (++chainFireNumber == CHAIN_FIRE_NUMBERS) {
					gunState = GunState.Idle;
					return;
				}

				startChainTime = Time.time;
				Fire(leftGunPosition, rightGunPosition);
				SoundController.instance.PlayerFire();
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
			rigidBody.AddForce(transform.forward * SHIP_SPEED, ForceMode.Impulse);
		}

		// Brake
		if (Input.GetKey(KeyCode.S)) {
			Brake();
		}

		// Right incline
		if (Input.GetKey(KeyCode.D)) {
			transform.RotateAround(transform.position, transform.forward, -1f);
		}

		// Left incline
		if (Input.GetKey(KeyCode.A)) {
			transform.RotateAround(transform.position, transform.forward, 1f);
		}		
	}


	void Brake()
	{
		if (Mathf.Abs(rigidBody.velocity.magnitude) > SHIP_STOPPED_SPEED) {
			rigidBody.AddForce(-rigidBody.velocity.normalized * SHIP_SPEED, ForceMode.Impulse);
			return;
		}
		rigidBody.velocity = Vector3.zero;
	}




	private void HandleMouseDirections()
	{
		//float dx = Input.GetAxis("Mouse X") * MOUSE_AMPLIFIER;
		//float dy = Input.GetAxis("Mouse Y") * MOUSE_AMPLIFIER;
		//transform.RotateAround(transform.position, transform.up, dx);
		//transform.RotateAround(transform.position, transform.right, -dy);

//		if (Input.GetMouseButtonDown(2)) {
//			Vector3 mousePosWorld = Vector3.zero;
//
//			Plane plane = new Plane(transform.up, transform.position);
//			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			float distance;
//			if (plane.Raycast(ray, out distance)) {
//				mousePosWorld = ray.GetPoint(distance);
//			}
//
//			//Debug.DrawRay(Camera.main.transform.position, mousePosWorld);
//
//
//			//transform.rotation = Quaternion.FromToRotation(transform.forward, newDirection);
//		}




		//Debug.DrawRay(transform.position, transform.up);

		//if (Input.GetKeyDown(KeyCode.E)) {
			var direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
			transform.rotation = Quaternion.LookRotation(direction);
		//}

		//if (Input.GetKey(KeyCode.Q)) {
			cameraActions.Invoke();
		//}

		//Debug.DrawRay(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 10f);

	}

}
