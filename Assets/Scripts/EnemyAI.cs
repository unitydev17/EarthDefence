using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

	public GameObject markerGreenPrefab;
	public GameObject markerRedPrefab;
	public GameObject bulletPrefab;

	private const float ATTACK_DISTANCE = 100f;
	private const float ROTATION_SPEED = 1f;
	private const float CHECK_POINT_SIZE = 3f;
	private const float FIRE_FREQUENCY = 1f;
	private const float BULLET_VELOCITY = 200f;
	private const float MOVE_VELOCITY = 0.1f;




	private enum State
	{
		Idle,
		Commanded,
		Flight,
		TargetVisible,
		Fire
	}


	private GameObject player;

	private State state;

	private Vector3 nearestAttackPoint;
	private Path path;
	private float time;

	private bool isStraightVisibility = false;



	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		state = State.Idle;
		path = new Path ();
		GameController.eventBus += ProcessCommand;
	}


	public void ProcessCommand (string command)
	{
		if (GameController.ATTACK_COMMAND == command) {
			state = State.Commanded;
		}
	}


	void Update ()
	{
		ProcessStates ();
	}



	private void ProcessStates ()
	{

		if (State.Commanded == state) {
			if (!CheckStraightVisibility ()) {
				path = FindPathToTarget (player, false);
				if (path != null) {
					path.Start ();
					state = State.Flight;
				}
			}
		}

		// only if player exists

		if (!CheckPlayerDestroyed ()) {

			if (State.Flight == state) {
				CheckStraightVisibility ();
				UpdateShipMovements ();
			}

			if (State.TargetVisible == state) {
				CheckStraightVisibility ();
				CheckCanFire ();
				UpdateShipMovements ();
			}


			if (State.Fire == state) {
				CheckPlayerDestroyed ();
				CheckStraightVisibility ();
				UpdateShipMovements ();

				float deltaTime = Time.time - time;
				if (deltaTime > FIRE_FREQUENCY) {
					Fire ();
					time = Time.time;
				}
			}
		}

	}

	void Fire() {
		Vector3 bulletSpawnPosition = transform.position + transform.forward * PlayerController.BULLET_SPAWN_DISTANCE;
		GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);
		Rigidbody bulletRig = bulletClone.GetComponent<Rigidbody>();
		bulletRig.velocity = transform.forward * BULLET_VELOCITY;
	}

	bool CheckPlayerDestroyed ()
	{
		if (player == null) {
			state = State.Idle;
			return true;
		}
		return false;
	}

	void CheckCanFire() {
		float distance = Vector3.Distance (transform.position, player.transform.position);
		if (distance <= ATTACK_DISTANCE) {

			Vector3 direction = transform.forward;
			distance = Vector3.Distance (transform.position, player.transform.position);
			RaycastHit hit;
			if (Physics.Raycast (transform.position, direction, out hit, distance) && hit.collider.gameObject == player) {
				state = State.Fire;
			}
		}
	}


	/// <summary>
	/// Check whether final point is seeing from the current position.
	/// if yes, make it as destination point.
	/// </summary>
	bool CheckStraightVisibility() {
		Vector3 direction = player.transform.position - transform.position;
		float distance = Vector3.Distance (transform.position, player.transform.position);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, direction, out hit, distance) && hit.collider.gameObject == player) {
			path.SetStraightPath(player.transform.position);
			state = State.TargetVisible;
			isStraightVisibility = true;
			return true;
		}

		// if target was lost make command again
		if (isStraightVisibility) {
			state = State.Commanded;
			isStraightVisibility = false;
		}
		return false;
	}

	void FlightMode ()
	{
		if (path != null) {
			path.Start ();
			state = State.Flight;
		}
	}

	void UpdateShipMovements ()
	{
		Vector3 direction = path.GetCurrentPoint () - transform.position;
		Debug.DrawRay (transform.position, direction);

		//rotate
		Vector3 newDir = Vector3.RotateTowards (transform.forward, direction, Time.deltaTime * ROTATION_SPEED, 0f);
		transform.rotation = Quaternion.LookRotation (newDir);
		// move
		GetComponent<Rigidbody> ().AddForce (direction.normalized * MOVE_VELOCITY, ForceMode.Impulse);

		if (State.TargetVisible != state) {
			if (Vector3.Distance (transform.position, path.GetCurrentPoint ()) < CHECK_POINT_SIZE) {
				path = FindPathToTarget (player, true);
				FlightMode ();
			}
		}

	}

	private Path FindPathToTarget(GameObject target, bool findNearest) {
		Path[] paths = PathFinder.FindPath (transform, target.transform);
		if (paths.Length > 0) {
			if (findNearest) {
				return PathFinder.GetNearestPath (paths, transform.position);
			}
			return paths [(int)(Random.value * (paths.Length - 1))];
		}
		return null;
	}

	private void DrawPath (Path path)
	{
		path.points.ForEach (point => {
			Instantiate (markerGreenPrefab, point, Quaternion.identity);
		});
	}

}
