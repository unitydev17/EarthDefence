using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathOld
{

	public static PathOld Empty = new PathOld();

	public static int counter = 0;

	public int id;
	public List<Vector3> points;
	private Vector3[] pointArray;
	private int currentPointNum;


	public void Init()
	{
		id = counter++;
		points = new List<Vector3>();
	}


	public PathOld()
	{
		Init();
	}


	public PathOld(Vector3 point)
	{
		Init();
		Add(point);
	}


	public void Add(Vector3 point)
	{
		points.Add(point);
	}


	public void Start()
	{
		pointArray = points.ToArray();
		currentPointNum = 0;
	}


	public Vector3 GetCurrentPoint()
	{
		return pointArray[currentPointNum];
	}


	public void NextPoint()
	{
		if (currentPointNum == pointArray.Length - 1) {
			return;
		}
		currentPointNum++;
	}


	public void ResetPath()
	{
		currentPointNum = 0;
		if (points == null) {
			points = new List<Vector3>();
		} else {
			points.Clear();
		}
	}


	public void SetStraightPath(Vector3 point)
	{
		ResetPath();
		Add(point);
		Start();
	}

	public bool IsEmpty() {
		return points.Count == 0;
	}
}
