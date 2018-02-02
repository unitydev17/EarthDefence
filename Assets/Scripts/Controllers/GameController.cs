using System;
using UnityEngine;

public class GameController : MonoBehaviour {

	public const string ATTACK_COMMAND = "Attack";
	private const int ENEMIES_COUNT = 10;
	private const float SPAWN_RADIUS = 20f;

	public static GameController instance;
	public static event Action<string> eventBus;

	public GameObject enemyPrefab;
	private Vector3 SPAWN_POINT = new Vector3 (100, 0, 100);


	private void Awake() {
		if (instance == null) {
			instance = this;
		}
	}


	void Start() {
		SpawnEnemies ();
		Cursor.visible = false;
	}

	void SpawnEnemies() {
		float deltaAngle = 360f / ENEMIES_COUNT;
		float angle = 0;
		for (int i = 0; i < ENEMIES_COUNT; i++) {
			Vector3 rotatedVector = Quaternion.AngleAxis (angle, Vector3.up) * new Vector3(0, i, SPAWN_RADIUS);
			Vector3 position = SPAWN_POINT + rotatedVector;
			Instantiate(enemyPrefab, position, Quaternion.LookRotation(Vector3.zero - SPAWN_POINT));
			angle += deltaAngle;
		}
	}


	private void Update() {
		HandleInput ();
	}


	private void HandleInput() {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			eventBus(ATTACK_COMMAND);
		}
	}

}
