using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayShoot : MonoBehaviour {
	private Camera _camera;
	// Use this for initialization
	void Start () {
		_camera = GetComponent<Camera> ();
		// hide cursor ESC to exit if playmode
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void OnGUI(){
		int size = 40;
		// put in the center
		float posX = _camera.pixelWidth / 2 - size / 4;
		float posY = _camera.pixelHeight / 2 - size / 2;
		GUI.Label (new Rect (posX, posY, size, size), "*");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
