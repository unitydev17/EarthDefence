using System.Collections.Generic;
using System;
using UnityEngine;

public class GameController : MonoBehaviour {

	public const string EARTH_NAME = "Earth";
	public const string MOON_NAME = "Moon";
	public const string MARS_NAME = "Mars";
	public const string PLAYER_TAG = "Player";
	public const float EARTH_MOON_DISTANCE = 100f;
	public const string PLANET_TAG = "Planet";
	public const string ATTACK_COMMAND = "Attack";
	public const string EXPLOSION_IMPACT_COMMAND = "ExplosionImpact";
	private const int MAX_ENEMIES_COUNT = 30;
	private const int ENEMIES_COUNT = 1;
	private const float SPAWN_RADIUS = 20f;
	private const float SPAWN_TIME = 5f;

	public static GameController instance;
	public static event Action<string, object> eventBus;

	public GameObject enemyPrefab;
	private Vector3 SPAWN_POINT = new Vector3 (0, 50, 0);

	private List<GameObject> enemies;

	public static GameObject root;
	private GameObject earth;
	private GameObject mars;
	private GameObject moon;
	private GameObject player;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		enemies = new List<GameObject>();
	}


	void Start() {
		root = new GameObject("Root");
		SetupPlanets();
		PlaceMoon();
		PlacePlayer();
		InvokeRepeating ("SpawnEnemies", SPAWN_TIME, SPAWN_TIME);
	}


	void PlacePlayer() {
		player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
		player.transform.position = (earth.transform.position + moon.transform.position) / 2f;
	}


	void PlaceMoon() {
		moon.transform.position = earth.transform.position + RandomizePosition(earth.transform, moon.transform, EARTH_MOON_DISTANCE);
	}


	void SetupPlanets() {
		GameObject[] planets = GameObject.FindGameObjectsWithTag(PLANET_TAG);
		foreach (GameObject planet in planets) {
			planet.transform.position = RandomizePosition(planet.transform, planet.transform.position.z);
			if (EARTH_NAME == planet.name) {
				earth = planet;
			} else if (MOON_NAME == planet.name) {
				moon = planet;
			} else if (MARS_NAME == planet.name) {
				mars = planet;
			}
		}
	}


	Vector3 RandomizePosition(Transform transform, float distance) {
		float xPos = RandomSign() * UnityEngine.Random.Range(-distance, distance);
		float zPos = RandomSign() * UnityEngine.Random.Range(-distance, distance);
		Vector3 result = new Vector3(xPos, 0, zPos).normalized * distance;
		result.y = transform.position.y;
		return result;
	}


	Vector3 RandomizePosition(Transform targetTransform, Transform transform, float distance) {
		float xPos = targetTransform.position.x + RandomSign() * UnityEngine.Random.Range(-distance, distance);
		float zPos = targetTransform.position.z + RandomSign() * UnityEngine.Random.Range(-distance, distance);
		Vector3 result = new Vector3(xPos, 0, zPos).normalized * distance;
		result.y = transform.position.y;
		return result;
	}


	int RandomSign()
	{
		return (UnityEngine.Random.value < 0.5f ? 1 : -1);
	}


	void SpawnEnemies() {
		if (enemies.Count > MAX_ENEMIES_COUNT) {
			return;
		}

		float deltaAngle = 360f / ENEMIES_COUNT;
		float angle = 0;
		for (int i = 0; i < ENEMIES_COUNT; i++) {
			Vector3 rotatedVector = Quaternion.AngleAxis (angle, Vector3.up) * new Vector3(0, i, SPAWN_RADIUS);
			Vector3 position = mars.transform.position + SPAWN_POINT + rotatedVector;
			GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.LookRotation(Vector3.zero - SPAWN_POINT));
			enemy.transform.parent = root.transform;

			//enemy.GetComponent<EnemyAI>().AttackCommand();

			enemies.Add(enemy);
			angle += deltaAngle;
		}
	}


	private void Update() {
		HandleInput ();
	}


	private void HandleInput() {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			eventBus(ATTACK_COMMAND, null);
		}
	}


	public static void ExplosionImpact(Vector3 center) {
		eventBus(EXPLOSION_IMPACT_COMMAND, center);
	}

	public void RemoveEnemy(GameObject gameObject) {
		if (enemies.Count > 0) {
			enemies.Remove(gameObject);
		}
	}
}
