using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAIOld : CommonShipController
{

	private const string PLAYER_TAG = "Player";
	public const float ATTACK_DISTANCE = 100f;
	public const float MIN_ATTACK_DISTANCE = 30f;
	private const float ROTATION_SPEED = 1f;
	private const float CHECK_POINT_SIZE = 3f;
	private const float FIRE_FREQUENCY = 1f;
	private const float MOVE_VELOCITY = 1f;
	// impulse mode

	private Vector3 rightGunPosition = new Vector3(1.967f, 0.276f, 2f);
	private Vector3 leftGunPosition = new Vector3(-1.967f, 0.276f, 2f);


	private enum State
	{
		Idle,
		FindAndAttack,
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
		state = State.FindAndAttack;
		path = new Path();
		isStraightVisibility = false;
	}


	public override void ProcessCommand(string command, object param)
	{
		base.ProcessCommand(command, param);

		if (GameController.ATTACK_COMMAND == command) {
			state = State.FindAndAttack;
		}
	}


	private void Update()
	{
		//if (State.Idle != state && !path.IsEmpty()) {
		//	Debug.DrawRay(transform.position, path.GetCurrentPoint());
		//}
		if (player) {
			ProcessStates();
		}
	}


	private void ProcessStates()
	{
		if (State.FindAndAttack == state) {
			if (!CheckStraightVisibility()) {
				//PathFinder.FindPathFollow(transform, player.transform, ref path, MIN_ATTACK_DISTANCE);
				path.Start();
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
				//CheckStraightVisibility();
				CheckCanFire();
				UpdateShipMovements();
			}

			if (State.Fire == state) {
				//CheckStraightVisibility();
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
			/*
			if (PathFinder.Instance.IsTargetVisible(transform.position, player.transform)) {
				state = State.Fire;
			}*/
		}
	}


	/// <summary>
	/// Check whether target is visible from the current position.
	/// if yes, make it as destination point.
	/// </summary>
	private bool CheckStraightVisibility()
	{
		/*
		if (PathFinder.Instance.IsTargetVisible(transform.position, player.transform)) {
			PathFinder.Instance.FindPathFollow(transform, player.transform, ref path, MIN_ATTACK_DISTANCE);
			path.Start();
			state = State.TargetVisible;
			isStraightVisibility = true;
			return true;
		}
		*/

		// if target was lost make command again
		if (isStraightVisibility) {
			state = State.FindAndAttack;
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
		MoveAndRotate();

		if (State.TargetVisible != state) {
			if (Vector3.Distance(transform.position, path.GetCurrent()) < CHECK_POINT_SIZE) {
				//PathFinder.Instance.FindPath(transform, player.transform, ref path);
				path.Start();
				FlightMode();
			}
		}
	}


	void MoveAndRotate()
	{
		Vector3 direction = path.GetCurrent() - transform.position;

		//rotate
		Vector3 newDir = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * ROTATION_SPEED, 0f);
		transform.rotation = Quaternion.LookRotation(newDir);

		// move
		float targetDistance = Vector3.Distance(transform.position, player.transform.position);
		if (targetDistance > MIN_ATTACK_DISTANCE) {
			rigidBody.AddForce(direction.normalized * MOVE_VELOCITY, ForceMode.Impulse);
			Debug.Log(Time.time + ": Force bot!");
		}

		ApplyMoveRestrictions();
	}


	public void AttackCommand()
	{
		state = State.FindAndAttack;
	}
}