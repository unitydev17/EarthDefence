using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class PathFinder
{
	#region CONSTANTS

	private const float WAYPOINT_SECTION_RADIUS = 0.5f;
	private const int TRACE_SEGMENTS = 16;
	private const float MAX_AMPLITUDE = 128f;
	private const int LAYER_IGNORE_RAYCAST = 2;
	private const int LAYER_DEFAULT = 0;
	private const int BACKWARD_RANDOM_SEARCH_ITERATIONS = 10;
	private const int BACKWARD_RADIUS = 10;

	#endregion

	/*
	#region SINGLETON

	private static volatile PathFinder instance;
	private static object syncRoot = new Object();


	private PathFinder()
	{
	}


	public static PathFinder Instance {
		get {
			if (instance == null) {
				lock (syncRoot) {
					if (instance == null) {
						instance = new PathFinder();
					}
				}
			}
			return instance;
		}
	}

	#endregion
	*/

	#region FIELDS



	private LinkedList<Vector3> wayPoints = new LinkedList<Vector3>();
	private int collidersToExclude;
	private SortedList<float, Vector3> values = new SortedList<float, Vector3>();

	#endregion


	public bool IsLookAtTarget(Transform from, Transform to) {
		Vector3 direction = to.position - from.position;
		RaycastHit hit;
		if (Physics.Raycast(from.position, direction, out hit, ItemAI.ATTACK_RADIUS)) {
			if (hit.transform.Equals(to)) {
				return true;
			}
		}
		return false;
	}


	public bool FindPathFollow(Transform from, Transform to, ref Path path, float radius)
	{
		float distance = Vector3.Distance(from.position, to.position);
		if (distance <= radius) {
			return true;
		}

		if (FindPath(from, to, ref path)) {
			LinkedList<Vector3> wayPoints = path.GetWayPoints();
			bool distanceReached = false;
			do {
				LinkedListNode<Vector3> node = wayPoints.Last;
			
				distance = 0;
				distance = Vector3.Distance(to.transform.position, node.Value); 

				if (distance < radius) {
					wayPoints.RemoveLast();
					if (wayPoints.Count == 0) {
						distanceReached = true;
					}
				} else {
					distanceReached = true;
				}
			} while (!distanceReached);

			path.Start();
			return true;
		}
		return false;
	}


	public bool FindPathAround(Transform from, Transform to, ref Path path, float radius)
	{
		Vector3 center = to.position;
		Vector3 point = center + new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized * radius;
		return FindPath(from, point, ref path);
	}


	public bool FindPathAround(Transform from, Vector3 to, ref Path path, float radius)
	{
		Vector3 center = to;
		Vector3 point = center + new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized * radius;
		return FindPath(from, point, ref path);
	}


	public bool FindPath(Transform from, Transform to, ref Path path)
	{
		from.gameObject.layer = LAYER_IGNORE_RAYCAST;
		collidersToExclude = 1;
		bool result = Find(from.position, to.position, ref path);
		from.gameObject.layer = LAYER_DEFAULT;
		return result;
	}


	public bool FindPath(Transform from, Vector3 to, ref Path path)
	{
		from.gameObject.layer = LAYER_IGNORE_RAYCAST;
		collidersToExclude = 0;
		bool result = Find(from.position, to, ref path);
		from.gameObject.layer = LAYER_DEFAULT;
		return result;
	}


	public bool FindPath(Vector3 from, Vector3 to, ref Path path)
	{
		collidersToExclude = 0;
		return Find(from, to, ref path);
	}


	private bool Find(Vector3 start, Vector3 finish, ref Path path)
	{	
		//float startTime = Time.realtimeSinceStartup;

		bool pathFound = FindForward(start, finish, ref path);
		if (!pathFound) {
			pathFound = FindBackward(start, finish, ref path);
		}

		//float duration = Time.realtimeSinceStartup - startTime;
		//Debug.Log(duration);
		return pathFound;
	}


	private bool FindBackward(Vector3 start, Vector3 finish, ref Path path)
	{
		var quaternion = Quaternion.LookRotation(start - finish);
		values.Clear();

		for (int i = 0; i < BACKWARD_RANDOM_SEARCH_ITERATIONS; i++) {
			float x = Random.Range(-BACKWARD_RADIUS, BACKWARD_RADIUS);
			float y = Random.Range(-BACKWARD_RADIUS, BACKWARD_RADIUS);
			float z = Random.Range(-BACKWARD_RADIUS, BACKWARD_RADIUS);

			Vector3 point = start + quaternion * new Vector3(x, y, z);

			Ray ray = new Ray();
			ray.direction = point - start;
			ray.origin = start;
			RaycastHit[] hits = Physics.SphereCastAll(ray, WAYPOINT_SECTION_RADIUS, Vector3.Distance(point, start));
			if (hits.Length == 0) {

				Debug.DrawLine(point, point * 1.05f);

				if (FindForward(point, finish, ref path)) {
					path.SetFirstWaypoint(point);
					float value = path.GetValue();
					if (!values.ContainsKey(value)) {
						values.Add(path.GetValue(), point);
					}
				}
			}
		}

		if (values.Count > 0) {
			var enumerator = values.GetEnumerator();
			enumerator.MoveNext();
			KeyValuePair<float, Vector3> pair = enumerator.Current;
			Vector3 bestPoint = (Vector3)pair.Value;

			FindForward(bestPoint, finish, ref path);
			path.SetFirstWaypoint(bestPoint);
			return true;
		}
		return false;
	}


	private bool FindForward(Vector3 start, Vector3 finish, ref Path path)
	{
		float period = 2f * Mathf.PI;
		float delta = period / TRACE_SEGMENTS;
		List<LinkedList<Vector3>> paths = new List<LinkedList<Vector3>>();

		// enlarge amplitude of the ellipsoid
		for (int amplitude = 1; amplitude < MAX_AMPLITUDE; amplitude++) {

			// rotate around the axis
			for (float phi = 0f; phi < period; phi += delta) {
				if (CheckPathClear(start, finish, amplitude, phi)) {
					paths.Add(new LinkedList<Vector3>(wayPoints));
				}
			}

			// randomize path selection
			if (paths.Count > 0) {
				int selectedPath = Random.Range(0, paths.Count);
				path.SetWayPoints(paths[selectedPath]);
				return true;
			}
		}
		return false;
	}


	private bool CheckPathClear(Vector3 start, Vector3 finish, float amplitude, float phi)
	{
		wayPoints.Clear();
		GetPoints(start, finish, amplitude, phi);
		return !HasInterceptions();
	}


	private bool HasInterceptions()
	{
		if (wayPoints.Count > 0) {
			IEnumerator enumerator = wayPoints.GetEnumerator();
			enumerator.MoveNext();
			Vector3 first = (Vector3)enumerator.Current;
			Vector3 second;

			HashSet<int> idList = new HashSet<int>();

			while (enumerator.MoveNext()) {
				second = (Vector3)enumerator.Current;

				Ray ray = new Ray();
				ray.direction = second - first;
				ray.origin = first;
				RaycastHit[] hits = Physics.SphereCastAll(ray, WAYPOINT_SECTION_RADIUS, Vector3.Distance(first, second), 1);

				foreach (RaycastHit hit in hits) {
					idList.Add(hit.transform.gameObject.GetInstanceID());
				}
					
				//Debug.DrawLine(first, second);

				first = second;
			}

			// There are several scenarios.
			// 1) User passed "transform from" and "transform to" objects to find path between them. In this case one (target transform) collider
			// should be excluded from the cast. 
			// 2) User passed "transform from" and "vector3 to" objects. There are no excluded colliders in this case.
			// 3) User passed "vector3 from" and "vector3 to" objects. There are no excluded colliders in this case. 
			return idList.Count > collidersToExclude;
		}
		return true;
	}


	private void GetPoints(Vector3 start, Vector3 finish, float amplitude, float phi)
	{
		var quaternion = Quaternion.LookRotation(start - finish);
		Vector3 center = (start + finish) / 2f;
		float dist = Vector3.Distance(start, finish);
		float rad = dist / 2f;
		float theta = 0;
		int steps = Mathf.RoundToInt(rad/2);
		float deltaT = Mathf.PI / steps;

		while (steps-- >= 0) {
			theta += deltaT;
			var tmp = amplitude * Mathf.Sin(theta);
			float x = tmp * Mathf.Cos(phi);
			float y = tmp * Mathf.Sin(phi);
			float z = rad * Mathf.Cos(theta);
			Vector3 point = center + quaternion * new Vector3(x, y, z);
			wayPoints.AddLast(point);
		}
	}

	public bool IsTargetVisible(Vector3 start, Transform target) {
		Vector3 direction = target.position - start;
		float distance = Vector3.Distance (start, target.position);
		RaycastHit hit;
		if (Physics.Raycast (start, direction, out hit, distance) && hit.collider.gameObject == target.gameObject) {
			return true;
		}
		return false;
	}

}
