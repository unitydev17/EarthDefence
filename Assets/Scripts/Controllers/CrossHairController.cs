using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CrossHairController : MonoBehaviour
{
	private const float SPEED_X = 10f;
	private const float SPEED_Y = 10f;
	private const float RETURN_SPEED = 2f;


	private static float dx, dy;
	public static Vector2 position;
	private Vector2 centerPosition;
	public static bool isEnabled;

	public GameObject crossHairStatic;
	private GameObject crossHairDynamic;


	void Awake() {
		crossHairDynamic = gameObject;
		PlayerController.playerEvents += ProcessEvents;
		MasterAI.masterAIEvents += ProcessEvents;
	}

	private void ProcessEvents(string command, object param)
	{
		if (GameController.GAME_OVER_EVENT == command) {
			crossHairStatic.SetActive (false);
			crossHairDynamic.SetActive (false);
		}
	}


	void Start()
	{
		isEnabled = true;
		Cursor.visible = false;
		centerPosition = new Vector3(Screen.width / 2, Screen.height / 2);
		position = centerPosition;
	}


	void Update()
	{
		if (isEnabled) {
			HandleMouseDirections ();
			UpdateTransform ();
		}
	}


	void UpdateTransform()
	{
		position = Vector2.Lerp(position, centerPosition, Time.deltaTime * RETURN_SPEED);
		transform.position = position;
	}


	private void HandleMouseDirections()
	{
		dx = Input.GetAxis("Mouse X") * SPEED_X;
		dy = Input.GetAxis("Mouse Y") * SPEED_Y;

		position = transform.position;
		position += new Vector2(dx, dy);
	}


	public static bool IsCrossHairMoved()
	{
		return !(dx == 0 && dy == 0);
	}


}
