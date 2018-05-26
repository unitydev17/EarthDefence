using System;
using System.Collections.Generic;
using UnityEngine;


public class DefenceStrategy : BaseStrategy
{

	private Transform defenceTarget;
	private float radius;
	private ItemAI itemAI;
	private PathFinder pathFinder;
	private float repeatStart;
	private float time;


	public DefenceStrategy(MonoBehaviour mono, Transform obj, Transform defenceTarget)
	{
		this.obj = obj;
		this.defenceTarget = defenceTarget;
		this.radius = GetRadius(defenceTarget) * 2f;
		state = State.CreatePath;
		itemAI = obj.GetComponent<ItemAI>();
		pathFinder = new PathFinder(mono);
		time = Time.time;
	}

	private float GetRadius(Transform transform) {
		var bounds = transform.GetComponent<MeshRenderer>().bounds;
		return bounds.extents.magnitude * transform.localScale.magnitude;
	}

	public override void Perform()
	{

		if (State.CreatePath == state) {
			CreatePathToDefendedTarget_Logic();
		}

		if (State.RepeatCreatePath == state) {
			RecreatePath_Logic();
		}

		if (State.MoveBase == state) {
			MoveToBase_Logic();
		}

		if (State.FollowAndAttack != state) {
			ObserveEnemies_Logic();
		}
			
		if (State.FollowAndAttack == state) {
			FollowEnemy_Logic();
		}

		if (State.FollowPath == state) {
			FollowPath_Logic();
		}

		CheckAttackFinished_Logic();

	}


	void CheckAttackFinished_Logic()
	{
		if (CheckStopAttack()) {
			state = State.CreatePath;
		}
	}


	void MoveToBase_Logic()
	{
		MoveRotate(path.GetCurrent());
	}


	void CreatePathToDefendedTarget_Logic()
	{
		repeatStart = Time.time;
		pathFinder.FindPathAround(obj, defenceTarget.position, radius, wayPoints => {
			if (wayPoints.Count > 0) {
				path.SetWayPoints(wayPoints);
				state = State.MoveBase;
			} else {
				state = State.RepeatCreatePath;
			}
		});
		state = State.WaitPathFind;
	}


	// Recreate path if last attempt failed
	void RecreatePath_Logic()
	{
		if ((Time.time - repeatStart) > REPEAT_PATH_WAIT) {
			state = State.CreatePath;
		}
	}


	// Find path to border violator and stand up to attack
	void ObserveEnemies_Logic()
	{
		if (CheckEnemyRadarDetect() && CheckEnemyVisible()) {
			state = State.FollowAndAttack;
		}
	}


	// Follow enemy and fire if possible
	void FollowEnemy_Logic()
	{
		// followed enemy has disappeared
		if (CheckEnemyInvisible()) {
			pathFinder.FindPath(obj, targetObj, wayPoints => {
				if (wayPoints.Count > 0) {
					path.SetWayPoints(wayPoints);
					state = State.FollowPath;
				} else {
					state = State.FollowAndAttack;
				}
			});
			state = State.WaitFollowPathFind;
			return;
		}

		// apply movements
		if (CheckAttackDistanceReached()) {
			RotateOnly(targetObj.position);
		} else {
			MoveRotate(targetObj.position);	
		}

		// shoot if possible
		if (CheckFirePossible()) {
			Fire();
		}
	}


	void FollowPath_Logic()
	{
		MoveRotate(path.GetCurrent());
	}


	private bool CheckStopAttack()
	{
		float distance = Vector3.Distance(obj.position, defenceTarget.position);
		return distance > ItemAI.ATTACK_STOP_DISTANCE;
	}


	private void Fire()
	{
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


	private bool CheckEnemyRadarDetect()
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


	private bool CheckEnemyVisible()
	{
		return pathFinder.IsTargetVisible(obj.position, targetObj);
	}


	private bool CheckEnemyInvisible()
	{
		return !CheckEnemyVisible();
	}

}