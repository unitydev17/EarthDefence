using System;
using UnityEngine;


public class BaseStrategy
{
	protected const float REPEAT_PATH_WAIT = 2f;

	protected enum State
	{
		CreatePath,
		RepeatCreatePath,
		WaitPathFind,
		MoveBase,
		FollowAndAttack,
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


	// Move and rotate
	public virtual void Perform()
	{
		if (State.MoveBase == state) {
			MoveRotate(path.GetCurrent());

		} else if (State.FollowAndAttack == state) {
			if (CheckStopMovement()) {
				RotateOnly(targetObj.position);
			} else {
				MoveRotate(targetObj.position);	
			}
		}

	}


	void RotateOnly(Vector3 toPoint)
	{
		Vector3 direction = toPoint - obj.position;
		Vector3 newDir = Vector3.RotateTowards(obj.forward, direction, Time.deltaTime * ItemAI.ROTATION_SPEED, 0f);
		obj.rotation = Quaternion.LookRotation(newDir);
	}


	void MoveRotate(Vector3 toPoint)
	{
		Vector3 direction = toPoint - obj.position;

		// rotate
		Vector3 newDir = Vector3.RotateTowards(obj.forward, direction, Time.deltaTime * ItemAI.ROTATION_SPEED, 0f);
		obj.rotation = Quaternion.LookRotation(newDir);

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
		float currDist = 0;
		try {
			currDist = Vector3.Distance(obj.position, path.GetCurrent());
		} catch (Exception e) {
			Debug.Log("exc");
		}

		if (currDist <= ItemAI.BOUND_RADIUS) {
			path.Next();
			CheckPathFinished();
		}
	}


	void CheckPathFinished()
	{
		if (path.IsFinished() && State.FollowAndAttack != state) {
			state = State.Idle;
		}
	}

	private bool CheckStopMovement()
	{
		float distance = Vector3.Distance(obj.position, targetObj.position);
		return distance <= ItemAI.ATTACK_RADIUS;
	}
}