using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class BaseStrategy
{
	protected const float REPEAT_PATH_WAIT = 2f;

	protected enum State
	{
		CreatePath,
		RepeatCreatePath,
		WaitPathFind,
		MoveBase,
		FollowAndAttack,
		WaitFollowPathFind,
		FollowPath,
		Idle
	}


	protected Transform targetObj;
	protected Transform obj;
	protected Path path;
	protected State state;


	public BaseStrategy()
	{
		path = new Path();
	}


	public Path GetPath()
	{
		return path;
	}



	public abstract void Perform();


	protected void RotateOnly(Vector3 toPoint)
	{
		Vector3 direction = toPoint - obj.position;
		Vector3 newDir = Vector3.RotateTowards(obj.forward, direction, Time.deltaTime * ItemAI.ROTATION_SPEED, 0f);
		obj.rotation = Quaternion.LookRotation(newDir);
	}


	protected void MoveRotate(Vector3 toPoint)
	{
		Vector3 direction = toPoint - obj.position;

		// rotate
		Vector3 newDir = Vector3.RotateTowards(obj.forward, direction, 1f, 0f);
		obj.rotation = Quaternion.Lerp(obj.rotation, Quaternion.LookRotation(newDir), Time.deltaTime * ItemAI.ROTATION_SPEED);

		if (!path.IsEmpty()) {
			// move
			Vector3 force = direction.normalized * ItemAI.FORCE;
			var rigidbody = obj.GetComponent<Rigidbody>();
			rigidbody.AddForce(force, ForceMode.Impulse);

			CheckNextPointAchieved();
		}
	}


	void CheckNextPointAchieved()
	{
		float currDist = Vector3.Distance(obj.position, path.GetCurrent());

		if (currDist <= ItemAI.BOUND_RADIUS) {
			path.Next();
			CheckPathFinished();
		}
	}


	void CheckPathFinished()
	{
		if (path.IsFinished()) {
			if (State.FollowPath == state) {
				state = State.FollowAndAttack;
			} else {
				state = State.Idle;
			}
		}
	}

	protected bool CheckAttackDistanceReached()
	{
		float distance = Vector3.Distance(obj.position, targetObj.position);
		return distance <= ItemAI.ATTACK_RADIUS;
	}

	protected abstract bool IsEnemy(string tag);


	#region DEBUG

	protected void DrawWay (LinkedList<Vector3> wayPoints)
	{
		if (wayPoints.Count == 0) {
			return;
		}

		IEnumerator enumerator = wayPoints.GetEnumerator ();
		enumerator.MoveNext ();
		Vector3 first = (Vector3)enumerator.Current;
		Vector3 second;
		while (enumerator.MoveNext ()) {
			second = (Vector3)enumerator.Current;
			Debug.DrawLine (first, second);
			first = second;
		}
	}

	#endregion
}