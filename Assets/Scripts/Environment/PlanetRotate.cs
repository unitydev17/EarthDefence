using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetRotate : MonoBehaviour
{

	public Transform targetRotation;

	public float targetRotationSpeed;
	public float selfRotationSpeed;


	void Update()
	{
		transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * selfRotationSpeed);
		if (targetRotation) {
			transform.RotateAround(targetRotation.position, Vector3.up, Time.deltaTime * targetRotationSpeed);
			transform.RotateAround(targetRotation.position, Vector3.right, Time.deltaTime * targetRotationSpeed);
		}
	}

}
