using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerFollower : MonoBehaviour
{

	private const float ROTATE_SPEED = 5f;

	public GameObject player;

	private Vector3 desiredPosition;
	private Quaternion desiredRotation;
	private Vector3 cameraOffset = new Vector3(0f, 1.5f, -4.5f);

	void Update() {
		if (player) {
			desiredPosition = player.transform.TransformPoint(cameraOffset);
			desiredRotation = player.transform.rotation;

			transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * ROTATE_SPEED);
			transform.position = desiredPosition;
		}

	}




}
