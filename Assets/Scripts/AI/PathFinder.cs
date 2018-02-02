using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{

	/// <summary>
	/// Finds a set of paths with two points inside. First point is the one visible from source object.
	/// Second point is the one visible from target point. 
	/// Path is so short because collider`s bounds used for points generation around the obstacle.
	/// </summary>
	/// <returns>The list of paths</returns>
	/// <param name="source">Source</param>
	/// <param name="target">Target</param>
	public static Path[] FindPath (Transform source, Transform target)
	{
		
		List<Path> paths = new List<Path> ();

		Vector3 direction = target.position - source.position;
		float distance = Vector3.Distance (source.position, target.position);

		RaycastHit hit;
		if (Physics.Raycast (source.position, direction, out hit, distance) && hit.collider.gameObject != target.gameObject) {
			Bounds bounds = hit.collider.bounds;

			// 1. Create a list of points around the obstacle
			bounds.Expand (5);
			var points = GenerateBoundedPoints (bounds);


			// 2. Process points nearest to source position
			List<Vector3> targetPoints = new List<Vector3> ();

			foreach(Vector3 point in points) {
				// visible from source object
				if (IsPointVisible(source.position, point)) {
					paths.Add (new Path (point));
				}

				// visible from target object
				if (IsPointVisible(target.position, point)) {
					targetPoints.Add (point);
				}
			}


			// find target point that is closest to source point and visible
			Vector3 nearestPoint;

			List<Path> pathsToRemove = new List<Path> ();

			foreach (Path path in paths) {
				nearestPoint = Vector3.zero;

				foreach (Vector3 point in path.points) {
					float minDistance = float.MaxValue;

					foreach (Vector3 targetPoint in targetPoints) {
						if (IsPointVisible(point, targetPoint)) {
							distance = Vector3.Distance (point, targetPoint);
							if (distance < minDistance) {
								minDistance = distance;
								nearestPoint = targetPoint;
							}
						}
					}
				}

				if (Vector3.zero == nearestPoint) {
					pathsToRemove.Add(path);
				} else {
					path.Add (nearestPoint);
					path.Add (target.transform.position);
				}
			}


			foreach (Path toRemove in pathsToRemove) {
				paths.Remove (toRemove);
			}

		}
		return paths.ToArray();
	}


	public static bool IsTargetVisible(Vector3 sourcePosition, Transform target) {
		Vector3 direction = target.position - sourcePosition;
		float distance = Vector3.Distance (sourcePosition, target.position);
		RaycastHit hit;
		if (Physics.Raycast (sourcePosition, direction, out hit, distance) && hit.collider.gameObject == target.gameObject) {
			return true;
		}
		return false;
	}


	private static bool IsPointVisible(Vector3 sourcePosition, Vector3 targetPosition) {
		Vector3 direction = targetPosition - sourcePosition;
		float distance = Vector3.Distance (sourcePosition, targetPosition);
		RaycastHit hit;
		if (!Physics.Raycast (sourcePosition, direction, out hit, distance)) {
			return true;
		}
		return false;
	}


	private static List<Vector3> GenerateBoundedPoints (Bounds bounds)
	{
		List<Vector3> points = new List<Vector3> ();
		points.Add (new Vector3 (bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
		//
		points.Add (new Vector3 (bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
		//
		points.Add (new Vector3 (bounds.center.x - bounds.extents.x, bounds.center.y, bounds.center.z - bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x - bounds.extents.x, bounds.center.y, bounds.center.z + bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x + bounds.extents.x, bounds.center.y, bounds.center.z + bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x + bounds.extents.x, bounds.center.y, bounds.center.z - bounds.extents.z));
		//
		points.Add (new Vector3 (bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z));
		points.Add (new Vector3 (bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z));
		points.Add (new Vector3 (bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z));
		points.Add (new Vector3 (bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z));
		//
		points.Add (new Vector3 (bounds.center.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
		points.Add (new Vector3 (bounds.center.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
		return points;
	}


	public static Path GetNearestPath(Path[] paths, Vector3 position) {
		float minPathLength = float.MaxValue;
		int minPathNumber = -1;
		for (int i = 0; i < paths.Length; i++) {
			float pathLength = 0;
			Vector3 startPoint = position;
			foreach (Vector3 point in paths[i].points) {
				pathLength += Vector3.Distance (startPoint, point);
				startPoint = point;
			}
			if (pathLength < minPathLength) {
				minPathLength = pathLength;
				minPathNumber = i;
			}
		}
		return paths [minPathNumber];
	}

}