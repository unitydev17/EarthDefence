using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerFollower : MonoBehaviour
{

	private enum State
	{
		Idle,
		Align
	}


	private State state;

	public GameObject player;

	private Vector3 desiredPosition;
	private Quaternion desiredRotation;
	private Vector3 cameraOffset = new Vector3(0f, 1.5f, -4.5f);

	private Coroutine alignCoroutine;

	public float smooth = 1;
	public float distanceUp = 2;
	public float distanceForward = 2;


	void Start()
	{
		state = State.Idle;
		PlayerController.cameraActions += AlignCamera;
	}


	void Update() {
		transform.GetComponent<Rigidbody>().velocity = player.GetComponent<Rigidbody>().velocity;
	}



	void LateUpdate3()
	{

		Vector3 targetPosition = player.transform.position + player.transform.up * distanceUp - player.transform.forward * distanceForward;
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
		transform.LookAt(player.transform);

	}



	void LateUpdate2()
	{

		if (State.Idle == state) {
		
		} else if (State.Align == state) {
			//transform.position = desiredPosition;

		}

	}


	public void AlignCamera()
	{
		if (alignCoroutine != null) {
			StopCoroutine(alignCoroutine);
		}


		state = State.Align;
		desiredPosition = player.transform.TransformPoint(cameraOffset);
		desiredRotation = player.transform.rotation;

		//transform.position = desiredPosition;
		//transform.rotation = desiredRotation;

		alignCoroutine = StartCoroutine(StraightCamera());
	
	}


	IEnumerator StraightCamera()
	{
		float maneurTime = 0.5f;
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;


		float dt;
		float startTime = Time.time;
		do {
			dt = Time.time - startTime;
			var complete = dt / maneurTime;

			transform.rotation = Quaternion.Lerp(rotation, desiredRotation, complete);
			transform.position = Vector3.Lerp(position, desiredPosition, complete);


			yield return null;
		} while (dt <= maneurTime);
	}



}
