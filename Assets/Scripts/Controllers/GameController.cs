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
	public const string GAME_OVER_EVENT = "GameOver";
	public const string GAME_WIN_EVENT = "GameWin";
	public const string HEALTH_UPDATE = "HealthUpdate";
	public const string EXPLOSION_IMPACT_COMMAND = "ExplosionImpact";
	public const float GAME_OVER_DELAY = 5f;
	public const float GAME_WIN_DELAY = 5f;


	public static GameController instance;
	public static event Action<string, object> eventBus;



	public static GameObject root;
	private GameObject earth;
	private GameObject moon;
	private GameObject player;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		root = new GameObject("Root");
	}


	void Start() {
		//root = new GameObject("Root");
		SetupPlanets();
		//PlaceMoon();
		//PlacePlayer();

		SoundController.instance.PlayBattleMusic ();
	}


	public static void UnsubscribeAll() {
		Delegate[] clientList = eventBus.GetInvocationList ();
		foreach (Delegate d in clientList) {
			eventBus -= (d as Action<string, object>);
		}
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
			//planet.transform.position = RandomizePosition(planet.transform, planet.transform.position.z);
			if (EARTH_NAME == planet.name) {
				earth = planet;
			} else if (MOON_NAME == planet.name) {
				moon = planet;
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

}
