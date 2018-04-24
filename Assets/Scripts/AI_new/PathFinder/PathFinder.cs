using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class PathFinder
{

	#region CONSTANTS

	private enum CollidersToExclude
	{
		None = 0,
		One = 1,
		Two = 2
	}


	private const float WAYPOINT_SECTION_RADIUS = 0.5f;
	private const int TRACE_SEGMENTS = 16;
	private const float MAX_AMPLITUDE = 128f;
	private const int LAYER_IGNORE_RAYCAST = 2;
	private const int LAYER_DEFAULT = 0;
	private const int BACKWARD_RANDOM_SEARCH_ITERATIONS = 10;
	private const int BACKWARD_RADIUS = 10;

	#endregion


	public PathFinder(MonoBehaviour mono)
	{
		this.mono = mono;
	}


	#region FIELDS



	private LinkedList<Vector3> wayPoints = new LinkedList<Vector3>();
	private SortedList<float, Vector3> values = new SortedList<float, Vector3>();
	private MonoBehaviour mono;
	private Coroutine coroutine;

	#endregion



	public void FindPathFollow(Transform from, Transform to, System.Action<LinkedList<Vector3>> callback)
	{
		FindPath(from, to.position, (wayPoints) => {
			callback(wayPoints);
		});
	}


	public void FindPathAround(Transform from, Vector3 to, float radius, System.Action<LinkedList<Vector3>> callback)
	{
		Vector3 center = to;
		Vector3 pointAround = center + new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized * radius;
		FindPath(from, pointAround, (wayPoints) => {
			callback(wayPoints);
		});
	}


	public void FindPath(Transform from, Vector3 to, System.Action<LinkedList<Vector3>> callback)
	{
		Find(CollidersToExclude.One, from.position, to, (wayPoints) => {
			callback(wayPoints);
		});
	}


	private void Find(CollidersToExclude collidersToExclude, Vector3 start, Vector3 finish, System.Action<LinkedList<Vector3>> callback)
	{	
		//float startTime = Time.realtimeSinceStartup;

		if (coroutine != null) {
			mono.StopCoroutine(coroutine);
		}

		coroutine = mono.StartCoroutine(FindForward(collidersToExclude, start, finish, wayPoints => {
			bool pathFound = wayPoints.Count > 0;
			if (pathFound) {
				callback(wayPoints);
			} else {
				//pathFound = FindBackward(start, finish, ref path);
				callback(wayPoints);
			}
		}));
			
		//float duration = Time.realtimeSinceStartup - startTime;
		//Debug.Log(duration);
	}

	/*
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
	}*/


	private IEnumerator FindForward(CollidersToExclude collidersToExclude, Vector3 start, Vector3 finish, System.Action<LinkedList<Vector3>> callback)
	{
		float period = 2f * Mathf.PI;
		float delta = period / TRACE_SEGMENTS;
		List<LinkedList<Vector3>> paths = new List<LinkedList<Vector3>>();

		// enlarge amplitude of the ellipsoid
		for (int amplitude = 1; amplitude < MAX_AMPLITUDE; amplitude++) {

			// rotate around the axis
			for (float phi = 0f; phi < period; phi += delta) {
				if (CheckPathClear(collidersToExclude, start, finish, amplitude, phi)) {
					paths.Add(new LinkedList<Vector3>(wayPoints));
				}
				yield return null;
			}

			// randomize path selection
			if (paths.Count > 0) {
				int selectedPath = Random.Range(0, paths.Count);
				callback(paths[selectedPath]);
				yield break;
			}
		}
		callback(Path.EMPTY);
	}


	private bool CheckPathClear(CollidersToExclude collidersToExclude, Vector3 start, Vector3 finish, float amplitude, float phi)
	{
		wayPoints.Clear();
		GetPoints(start, finish, amplitude, phi);
		return !HasInterceptions(collidersToExclude);
	}


	private bool HasInterceptions(CollidersToExclude collidersToExclude)
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
			// 1) User passed "transform from" and "transform to" objects to find path between them. In this case two (target transform) colliders
			// should be excluded from the cast. 
			// 2) User passed "transform from" and "vector3 to" objects. There is one excluded colliders in this case.
			// 3) User passed "vector3 from" and "vector3 to" objects. There are no excluded colliders in this case. 
			return idList.Count > (int)collidersToExclude;
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
		int steps = Mathf.RoundToInt(rad / 2);
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


	public bool IsTargetVisible(Vector3 start, Transform target)
	{
		Vector3 direction = target.position - start;
		float distance = Vector3.Distance(start, target.position);
		RaycastHit hit;
		if (Physics.Raycast(start, direction, out hit, distance) && hit.collider.gameObject == target.gameObject) {
			return true;
		}
		return false;
	}


	public bool IsLookAtTarget(Transform from, Transform to)
	{
		Vector3 direction = from.forward;
		RaycastHit hit;
		if (Physics.Raycast(from.position, direction, out hit, ItemAI.ATTACK_RADIUS)) {
			if (hit.transform.Equals(to)) {
				return true;
			}
		}
		return false;
	}

}
