using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseAI : MonoBehaviour
{
	public GameObject wayPointPrefab;

	private List<GameObject> debugBalls;


	void Awake() {
		debugBalls = new List<GameObject>();
	}


	protected void DebugPath(LinkedList<Vector3> points) {
		foreach (Vector3 point in points) {
			GameObject ball = Instantiate(wayPointPrefab, point, Quaternion.identity);
			//ball.transform.localScale = new Vector3(ItemAI.BOUND_RADIUS, ItemAI.BOUND_RADIUS, ItemAI.BOUND_RADIUS);
			debugBalls.Add(ball);
		}
	}

	public void RemoveBalls()
	{
		foreach (GameObject ball in debugBalls) {
			DestroyImmediate(ball);
		}
	}
}