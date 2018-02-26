using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CrossHairController : MonoBehaviour
{
	private const float SPEED_X = 10f;
	private const float SPEED_Y = 10f;
	private const float RETURN_DELAY = 15f;


	private static float dx, dy;
	public static Vector2 position;
	private Vector2 centerPosition;
	private Coroutine returnToCenterCoroutine;


	void Start()
	{
		Cursor.visible = false;
		centerPosition = new Vector3(Screen.width / 2, Screen.height / 2);
		position = centerPosition;
	}


	void Update()
	{
		HandleMouseDirections();

	}


	void LateUpdate()
	{
		transform.position = position;
	}


	private void HandleMouseDirections()
	{
		bool crossHairMoved = CheckMouseMoved();

		if (crossHairMoved) {
			if (returnToCenterCoroutine != null) {
				StopCoroutine(returnToCenterCoroutine);
				Debug.Log(Time.time + " stopCoroutine " + returnToCenterCoroutine.GetHashCode());
			}
			returnToCenterCoroutine = StartCoroutine(ReturnToCenter());
			Debug.Log(Time.time + " startCoroutine " + returnToCenterCoroutine.GetHashCode());
		}
	}


	IEnumerator ReturnToCenter()
	{
		float dt = 0;
		do {
			position = Vector2.Lerp(position, centerPosition, dt / RETURN_DELAY);
			dt += Time.deltaTime;

			yield return null;
		} while (dt < RETURN_DELAY);
	}

	/*
	IEnumerator ReturnToCenter() {
		float startTime = Time.time;
		float dt;

		do {
			dt = Time.time - startTime;
			Debug.Log("dt:" + dt + " st: " + startTime);
			position = Vector2.Lerp(position, centerPosition, dt/RETURN_DELAY);

			yield return null;
		} while (dt < RETURN_DELAY);
	}*/


	bool CheckMouseMoved()
	{
		dx = Input.GetAxis("Mouse X") * SPEED_X;
		dy = Input.GetAxis("Mouse Y") * SPEED_Y;
		position += new Vector2(dx, dy);
		return IsCrossHairMoved();
	}


	public static bool IsCrossHairMoved()
	{
		return !(dx == 0 && dy == 0);
	}


	void OnGUI()
	{
		GUI.TextArea(new Rect(10, 10, 200, 30), "(" + dx + ":" + dy + ")");
		GUI.TextArea(new Rect(10, 50, 200, 30), "(" + position.x + ":" + position.y + ")");
		GUI.TextArea(new Rect(10, 90, 200, 30), "(" + centerPosition + ")");
	}

}
