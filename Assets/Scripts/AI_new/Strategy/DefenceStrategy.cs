using System;
using System.Collections.Generic;
using UnityEngine;


public class DefenceStrategy : BaseStrategy
{

	private Vector3 target;
	private float radius;
	private ItemAI itemAI;
	private PathFinder pathFinder;
	private float repeatStart;
	private float time;


	public DefenceStrategy(Transform obj, Vector3 target, float radius)
	{
		this.obj = obj;
		this.target = target;
		this.radius = radius;
		state = State.CreatePath;
		itemAI = obj.GetComponent<ItemAI>();
		pathFinder = new PathFinder();
		time = Time.time;
	}


	public override void Perform()
	{

		// Create path in case enemy visible
		if (State.CreatePath == state) {
			repeatStart = Time.time;
			if (pathFinder.FindPathAround(obj, target, ref path, radius)) {
				Debug.Log("Create path");
				state = State.MoveBase;
			} else {
				state = State.RepeatCreatePath;
			}
		}


		// Recreate path if last attempt failed
		if (State.RepeatCreatePath == state) {
			if ((Time.time - repeatStart) > REPEAT_PATH_WAIT) {
				state = State.CreatePath;
			}
		}

		// Find path to border violator and stand up to attack
		if (State.FollowAndAttack != state && CheckAttackPossible()) {
			pathFinder.FindPathFollow(obj, targetObj, ref path, ItemAI.ATTACK_RADIUS);
			state = State.FollowAndAttack;
		}

		// Fire if possible
		if (State.FollowAndAttack == state) {

			if (CheckFirePossible()) {
				Fire();
			}

			if (CheckStopAttack()) {
				state = State.CreatePath;
			}
		}

		// Move and rotate
		base.Perform();
	}

	private bool CheckStopAttack() {
		float distance = Vector3.Distance(obj.position, target);
		bool attackStop = distance > ItemAI.ATTACK_STOP_DISTANCE;
		attackStop |= !CheckAttackPossible();
		return attackStop;
	}


	private void Fire() {
		float deltaTime = Time.time - time;
		if (deltaTime > ItemAI.FIRE_FREQUENCY) {
			itemAI.Fire();
			time = Time.time;
		}
	}


	private bool CheckFirePossible() 
	{
		float distance = Vector3.Distance(obj.position, targetObj.position);
		bool firePossible = distance <= ItemAI.ATTACK_RADIUS;
		firePossible &= pathFinder.IsLookAtTarget(obj, targetObj);
		return firePossible;
	}


	private bool CheckAttackPossible()
	{
		Collider[] colliders = Physics.OverlapSphere(obj.position, ItemAI.VIEW_RADIUS);
		foreach (Collider collider in colliders) {
			if (collider.tag == "Player" || collider.tag == "PlayerTeam") {
				targetObj = collider.transform;
				return true;
			}
		}
		return false;
	}

}