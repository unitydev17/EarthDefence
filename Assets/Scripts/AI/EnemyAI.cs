using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAI : CommonShipController
{

	private const string PLAYER_TAG = "Player";
	private const float ATTACK_DISTANCE = 100f;
	private const float MIN_ATTACK_DISTANCE = 20f;
	private const float ROTATION_SPEED = 1f;
	private const float CHECK_POINT_SIZE = 3f;
	private const float FIRE_FREQUENCY = 1f;
	private const float MOVE_VELOCITY = 0.5f; // impulse mode

	private Vector3 rightGunPosition = new Vector3(1.967f, 0.276f, 2f);
	private Vector3 leftGunPosition = new Vector3(-1.967f, 0.276f, 2f);


	private enum State
	{
		Idle,
		Commanded,
		Flight,
		TargetVisible,
		Fire
	}
		

	private State state;
	private Path path;
	private float time;
	private bool isStraightVisibility;


	protected override void Start()
	{
		base.Start();
		state = State.Idle;
		path = new Path();
		isStraightVisibility = false;
	}


	public override void ProcessCommand(string command, object param)
	{
		base.ProcessCommand(command, param);

		if (GameController.ATTACK_COMMAND == command) {
			state = State.Commanded;
		}
	}


	private void Update()
	{
		if (State.Idle != state && !path.IsEmpty()) {
			Debug.DrawRay(transform.position, path.GetCurrentPoint());
		}

		ProcessStates();
	}


	private void ProcessStates()
	{
		if (State.Commanded == state) {
			if (!CheckStraightVisibility()) {
				path = FindRandomPathToTarget(player);
				FlightMode();
			}
		}

		// only if player exists

		if (!CheckPlayerDestroyed()) {

			if (State.Flight == state) {
				CheckStraightVisibility();
				UpdateShipMovements();
			}

			if (State.TargetVisible == state) {
				CheckStraightVisibility();
				CheckCanFire();
				UpdateShipMovements();
			}

			if (State.Fire == state) {
				CheckStraightVisibility();
				UpdateShipMovements();

				float deltaTime = Time.time - time;
				if (deltaTime > FIRE_FREQUENCY) {
					if (!CheckPlayerDestroyed()) {
						Fire(leftGunPosition, rightGunPosition);
					}
					time = Time.time;
				}
			}
		}
	}


	bool CheckPlayerDestroyed()
	{
		if (player == null) {
			state = State.Idle;
			return true;
		}
		return false;
	}


	void CheckCanFire()
	{
		float distance = Vector3.Distance(transform.position, player.transform.position);
		if (distance <= ATTACK_DISTANCE) {
			if (PathFinder.IsTargetVisible(transform.position, player.transform)) {
				state = State.Fire;
			}
		}
	}


	/// <summary>
	/// Check whether target is visible from the current position.
	/// if yes, make it as destination point.
	/// </summary>
	private bool CheckStraightVisibility()
	{
		if (PathFinder.IsTargetVisible(transform.position, player.transform)) {
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


	void FlightMode()
	{
		if (!path.IsEmpty()) {
			path.Start();
			state = State.Flight;
		}
	}


	void UpdateShipMovements()
	{
		Vector3 direction = path.GetCurrentPoint() - transform.position;

		//rotate
		Vector3 newDir = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * ROTATION_SPEED, 0f);
		transform.rotation = Quaternion.LookRotation(newDir);

		// move
		float targetDistance = Vector3.Distance(transform.position, player.transform.position);
		if (targetDistance > MIN_ATTACK_DISTANCE) {
			GetComponent<Rigidbody>().AddForce(direction.normalized * MOVE_VELOCITY, ForceMode.Impulse);
		}

		if (State.TargetVisible != state) {
			if (Vector3.Distance(transform.position, path.GetCurrentPoint()) < CHECK_POINT_SIZE) {
				path = FindNearestPathToTarget(player);
				FlightMode();
			}
		}

	}


	private Path FindNearestPathToTarget(GameObject target)
	{
		return FindPathToTarget(target, true);
	}


	private Path FindRandomPathToTarget(GameObject target)
	{
		return FindPathToTarget(target, false);
	}


	/// <summary>
	/// Finds the path to target.
	/// </summary>
	/// <returns>The path to target.</returns>
	/// <param name="target">Target.</param>
	/// <param name="findNearest">If set to <c>true</c> find nearest path otherwise find random path.</param>
	private Path FindPathToTarget(GameObject target, bool findNearestPath)
	{
		Path[] paths = PathFinder.FindPath(transform, target.transform);
		if (paths.Length > 0) {
			if (findNearestPath) {
				return PathFinder.GetNearestPath(paths, transform.position);
			}
			return paths[(int)(Random.value * (paths.Length - 1))];
		}
		return Path.Empty;
	}

}