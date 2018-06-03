using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {

	public GameObject target;
	public bool isChase;

	void Start() {
		isChase = true;
	}

	void Update () {
		if (isChase) {
			transform.LookAt (target.transform.position);	
		}
	}
}
