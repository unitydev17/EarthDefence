using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class DefenceStrategy : BaseStrategy
{

	private Transform _defenceTarget;
	private float _radius;
	private ItemAI _itemAI;
	private PathFinder _pathFinder;
	private float _repeatStart;
	private float _time;


	public DefenceStrategy(MonoBehaviour mono, Transform obj, Transform defenceTarget)
	{
		this.obj = obj;
		this._defenceTarget = defenceTarget;
		this._radius = GetRadius(defenceTarget) * 2f;
		state = State.CreatePath;
		_itemAI = obj.GetComponent<ItemAI>();
		_pathFinder = new PathFinder(mono);
		_time = Time.time;
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

		if (State.MoveBase != state) {
			CheckAttackFinished_Logic ();
		}

		//DrawWay (path.GetWayPoints());
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
		_repeatStart = Time.time;
		state = State.WaitPathFind;
		_pathFinder.FindPathRandomTraectory(obj, _defenceTarget.position, _radius, wayPoints => {
			if (wayPoints.Count > 0) {
				path.SetWayPoints(wayPoints);
				state = State.MoveBase;
			} else {
				state = State.RepeatCreatePath;
			}
		});
	}


	// Recreate path if last attempt failed
	void RecreatePath_Logic()
	{
		if ((Time.time - _repeatStart) > REPEAT_PATH_WAIT) {
			state = State.CreatePath;
		}
	}


	// Find path to border violator and stand up to attack
	void ObserveEnemies_Logic()
	{
		if (CheckEnemyRadarDetect () && CheckEnemyVisible ()) {
			state = State.FollowAndAttack;
		}
	}


	// Follow enemy and fire if possible
	void FollowEnemy_Logic()
	{
		// followed enemy has disappeared
		if (CheckEnemyInvisible()) {
			state = State.WaitFollowPathFind;
			_pathFinder.FindPath(obj, targetObj, wayPoints => {
				if (wayPoints.Count > 0) {
					path.SetWayPoints(wayPoints);
					state = State.FollowPath;
				} else {
					state = State.FollowAndAttack;
				}
			});
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
		float distance = Vector3.Distance(obj.position, _defenceTarget.position);
		return distance > ItemAI.ATTACK_STOP_DISTANCE;
	}


	private void Fire()
	{
		float deltaTime = Time.time - _time;
		if (deltaTime > ItemAI.FIRE_FREQUENCY) {
			_itemAI.Fire();
			_time = Time.time;
		}
	}


	private bool CheckFirePossible()
	{
		float distance = Vector3.Distance(obj.position, targetObj.position);
		bool firePossible = distance <= ItemAI.ATTACK_RADIUS;
		firePossible &= _pathFinder.IsLookAtTarget(obj, targetObj);
		return firePossible;
	}


	private bool CheckEnemyRadarDetect()
	{
		Collider[] colliders = Physics.OverlapSphere(obj.position, ItemAI.VIEW_RADIUS);
		foreach (Collider collider in colliders) {
			if (IsEnemy(collider.tag)) {
				targetObj = collider.transform;
				return true;
			}
		}
		return false;
	}


	protected override bool IsEnemy (string tag)
	{
		return tag == CommonShipController.PLAYER_TAG || tag == CommonShipController.PLAYER_TEAM_TAG;
	}


	private bool CheckEnemyVisible()
	{
		return _pathFinder.IsTargetVisible(obj.position, targetObj);
	}


	private bool CheckEnemyInvisible()
	{
		return !CheckEnemyVisible();
	}

}