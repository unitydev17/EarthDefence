using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindAI : MonoBehaviour {

	public GameObject enemy;
	private PathFinder pathFinder;
	private bool tryOnce;


	// Use this for initialization
	void Start () {
		pathFinder = new PathFinder (this);
		tryOnce = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (tryOnce) {
			tryOnce = false;
			pathFinder.FindPath (transform, enemy.transform, wayPoints => {

				IEnumerator enumerator = wayPoints.GetEnumerator();
				enumerator.MoveNext();
				Vector3 first = (Vector3)enumerator.Current;
				Vector3 second;

				while(enumerator.MoveNext()) {
					second = (Vector3)enumerator.Current;	
					Debug.DrawLine(first, second);
					first = second;
				}


				tryOnce = true;
			});
		}
	}
}
