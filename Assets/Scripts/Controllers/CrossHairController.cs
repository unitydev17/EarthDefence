using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairController : MonoBehaviour {

	void Update() {
		HandleMouseDirections();

	}


	private void HandleMouseDirections()
	{
		/*
		Vector3 position = transform.position;
		float dx = Input.GetAxis("Mouse X");
		float dy = Input.GetAxis("Mouse Y");
		position += new Vector3(dx, dy, 0);
		transform.position = position;*/
		transform.position = Input.mousePosition;
	}

}
