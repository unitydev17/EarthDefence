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


	public DefenceStrategy(MonoBehaviour mono, Transform obj, Vector3 target, float radius)
	{
		this.obj = obj;
		this.target = target;
		this.radius = radius;
		state = State.CreatePath;
		itemAI = obj.GetComponent<ItemAI>();
		pathFinder = new PathFinder(mono);
		time = Time.time;
	}


	public override void Perform()
	{

		if (State.CreatePath == state) {
			CreatePathToDefendedTarget_Logic();
		}

		if (State.RepeatCreatePath == state) {
			RecreatePath_Logic();
		}

		if (State.FollowAndAttack != state) {
			ObserveEnemies_Logic();
		}
			
		if (State.FollowAndAttack == state) {
			FollowEnemy_Logic();
		}

		base.Perform();
	}


	void CreatePathToDefendedTarget_Logic()
	{
		repeatStart = Time.time;
		pathFinder.FindPathAround(obj, target, radius, wayPoints =>  {
			if (wayPoints.Count > 0) {
				path.SetWayPoints(wayPoints);
				state = State.MoveBase;
			}
			else {
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
		if (CheckEnemyVisible()) {
			//pathFinder.FindPathFollow(obj, targetObj, ref path, ItemAI.ATTACK_RADIUS);
			state = State.FollowAndAttack;
		}
	}


	// Follow enemy and fire if possible
	void FollowEnemy_Logic()
	{
		if (CheckFirePossible()) {
			Fire();
		}
		if (CheckStopAttack()) {
			state = State.CreatePath;
		}
	}


	private bool CheckStopAttack()
	{
		float distance = Vector3.Distance(obj.position, target);
		bool attackStop = distance > ItemAI.ATTACK_STOP_DISTANCE;
		attackStop |= CheckEnemyInvisible();
		return attackStop;
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


	private bool CheckEnemyVisible()
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


	private bool CheckEnemyInvisible()
	{
		return !CheckEnemyVisible();
	}

}