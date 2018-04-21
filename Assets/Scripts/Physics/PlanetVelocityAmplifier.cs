using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetVelocityAmplifier : MonoBehaviour {

	protected static GameObject[] planets;

	private float RAD_1 = 0;

	[SerializeField]
	private float SLOWAGE_DISTANCE = 5000;


	Rigidbody rigidbodyCached;

	void Awake() {
		rigidbodyCached = GetComponent<Rigidbody>();
	}


	void Start() {
		planets = GameObject.FindGameObjectsWithTag("Planet");
	}

	void Update() {
		CheckPlanetsDistance();
	}
		

	void CheckPlanetsDistance() {
		foreach (GameObject p in planets) {
			float dist = Vector3.Distance(transform.position, p.transform.position);
			float rad = p.transform.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * p.transform.localScale.x;

			float r1 = rad + RAD_1;
			float r2 = rad + SLOWAGE_DISTANCE;


			if (dist < r2) {

				float multiplier = (dist - r1) / (r2 - r1);
				float maxVelocity = PlayerController.MAX_VELOCITY * multiplier;
					
				if (rigidbodyCached.velocity.magnitude >= maxVelocity) {
						rigidbodyCached.velocity = maxVelocity * rigidbodyCached.velocity.normalized;
				}

			}

		}
	}

}
