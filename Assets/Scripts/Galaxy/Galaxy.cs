using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// to do 
// smooth movement up down left right
// improve controller
// different map asteroids
// improve sun
// collision

public class Galaxy : MonoBehaviour {
	public int numberStars = 300;
	public int minimumRadius = 90;
	public int maximumRadius = 300;
	public float minSize = 0.1f;
	public float maxSize = 10.0f;
	public float minDist = 8.0f;

	public GameObject Asteroid;

	// Use this for initialization
	void Start () {

		SafetyCheck ();

		int failCount = 0;
		for (int i = 0; i < numberStars; i++)
		{
			Vector3 positionAster = RandomPosition ();

			// for check overlapping 
			Collider[] positionCollider = Physics.OverlapSphere (
				                              positionAster, minDist);
			//
			if (positionCollider.Length == 0) {
				string name = "Asteroid" + i;
				CreateSphere (name, positionAster);
				failCount = 0;

			} else {
				i--;
				failCount++;
			}
			if (failCount > numberStars) {
				Debug.LogError ("minDist too big");
				break;
			}

		}
	}
	// check data for range min max stars before galaxy created
	void SafetyCheck()
	{
		if (minimumRadius > maximumRadius) {
			int temp = maximumRadius;
			maximumRadius = minimumRadius;
			minimumRadius = temp;
		}
	}
	// create random coordinates for stars
	Vector3 RandomPosition() {
		float distance = Random.Range(minimumRadius, maximumRadius);
		// in radians
		float angle = Random.Range(0, 2 * Mathf.PI);
		float height = Random.Range (minimumRadius, maximumRadius);
		float angleY = Random.Range(0, 2 * Mathf.PI);
		// position in 3d coords
		Vector3 positionAster = new Vector3 (
			distance * Mathf.Cos (angle), 
			height * Mathf.Cos (angleY), 
			distance * Mathf.Sin (angle));
		return positionAster;
	}


	// create sphere object
	void CreateSphere (string name, Vector3 positionStar)
	{
		float astScale = Random.Range (minSize, maxSize);
		GameObject astObj = Instantiate(Asteroid);
		astObj.transform.localScale = new Vector3 (astScale, astScale, astScale);
		astObj.transform.position = positionStar;
		astObj.name = name;

	}

	// Update is called once per frame
	void Update () {

	}
}
