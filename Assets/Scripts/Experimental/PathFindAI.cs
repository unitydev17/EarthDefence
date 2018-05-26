using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindAI : MonoBehaviour
{

	public GameObject enemy;
	private PathFinder pathFinder;
	private bool tryMore;


	void Start ()
	{
		#if UNITY_EDITOR
		UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
		#endif

		pathFinder = new PathFinder (this);
		tryMore = true;
	}


	void Update ()
	{
		if (tryMore) {
			tryMore = false;

			pathFinder.FindPath (transform, enemy.transform, wayPoints => {
				DrawWay (wayPoints);
				tryMore = true;
			});
		}
	}


	void DrawWay (LinkedList<Vector3> wayPoints)
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
}
