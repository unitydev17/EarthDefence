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
			transform.RotateAround(targetRotation.position, targetRotation.up, Time.deltaTime * targetRotationSpeed);
		}
	}

}
