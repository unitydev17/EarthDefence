using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Path
{

	private LinkedList<Vector3> wayPoints;
	private IEnumerator enumerator;
	private bool isFinished;


	public void SetWayPoints(LinkedList<Vector3> points)
	{
		this.wayPoints = points;
		Start();
	}


	public LinkedList<Vector3> GetWayPoints()
	{
		return wayPoints;
	}


	public Path()
	{
		wayPoints = new LinkedList<Vector3>();
		enumerator = wayPoints.GetEnumerator();
	}


	public void Next()
	{
		isFinished = !enumerator.MoveNext();
	}


	public Vector3 GetCurrent()
	{
		return (Vector3)enumerator.Current;
	}


	public bool IsFinished()
	{
		return isFinished;
	}


	public void SetFirstWaypoint(Vector3 point)
	{
		wayPoints.AddFirst(point);
		Start();
	}


	public float GetValue()
	{
		float value = 0;
		if (wayPoints.Count != 0) {

			IEnumerator enumerator = wayPoints.GetEnumerator();
			enumerator.MoveNext();
			Vector3 first = (Vector3)enumerator.Current;
			Vector3 second;
			while (enumerator.MoveNext()) {
				second = (Vector3)enumerator.Current;
				value += Vector3.Distance(first, second);
				first = second;
			}
		}
		return value;
	}


	public bool IsEmpty() {
		return wayPoints.Count == 0;
	}

	public void Start()
	{
		enumerator = wayPoints.GetEnumerator();
		enumerator.MoveNext();
	}

}