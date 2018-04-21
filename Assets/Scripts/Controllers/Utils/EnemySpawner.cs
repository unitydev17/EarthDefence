using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{

	[SerializeField]
	private int MAX_ENEMIES_COUNT = 1;

	[SerializeField]
	private int SPAWN_SQUAD_ENEMIES_COUNT = 1;

	[SerializeField]
	private float SPAWN_TIME = 5f;

	private const float SPAWN_RADIUS = 20f;

	public GameObject enemyPrefab;

	private Vector3 spawnPoint;


	void Start()
	{
		float rad = GetComponent<MeshFilter>().mesh.bounds.extents.x * transform.localScale.x;
		spawnPoint = new Vector3(0, 0, rad * 1.10f);
		InvokeRepeating("SpawnEnemies", SPAWN_TIME, SPAWN_TIME);
	}


	void SpawnEnemies()
	{
		//if (GameController.enemies.Count >= MAX_ENEMIES_COUNT) {
		//	return;
		//}

		float deltaAngle = 360f / SPAWN_SQUAD_ENEMIES_COUNT;
		float angle = 0;
		for (int i = 0; i < SPAWN_SQUAD_ENEMIES_COUNT; i++) {
			Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * new Vector3(0, i, SPAWN_RADIUS);
			Vector3 position = transform.position + spawnPoint + rotatedVector;
			GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
			enemy.transform.parent = GameController.root.transform;

		//	GameController.enemies.Add(enemy);
			angle += deltaAngle;
		}
	}
}
