using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pools
{

	private static Pools instance;
	private static readonly object locker = new object();
	public static Pools Instance {
		get {
			lock (locker) {
				if (instance == null) {
					instance = new Pools ();
				}
				return instance;
			}
		}
	}

	private GameObject enemyBulletPrefab;
	private GameObject playerBulletPrefab;
	private GameObject explosionPrefab;
	private GameObject enemyBotPrefab;
	private GameObject playerBotPrefab;


	public Pools() {
		playerBulletPrefab = Resources.Load (PLAYER_BULLET_PREFAB_PATH) as GameObject;
		enemyBulletPrefab = Resources.Load (ENEMY_BULLET_PREFAB_PATH) as GameObject;
		explosionPrefab = Resources.Load (EXPLOSION_BULLET_PREFAB_PATH) as GameObject;
		enemyBotPrefab = Resources.Load (ENEMY_BOT_PREFAB_PATH) as GameObject;
		playerBotPrefab = Resources.Load (PLAYER_BOT_PREFAB_PATH) as GameObject;
	}

	private const string ENEMY_BOT_PREFAB_PATH = "Prefabs/Enemy";
	private const string PLAYER_BOT_PREFAB_PATH = "Prefabs/PlayerItem";
	private const string ENEMY_BULLET_PREFAB_PATH = "Prefabs/Items/EnemyBullet";
	private const string PLAYER_BULLET_PREFAB_PATH = "Prefabs/Items/PlayerBullet";
	private const string EXPLOSION_BULLET_PREFAB_PATH = "Prefabs/VFX/Explosion3";

	private List<GameObject> enemyBullets = new List<GameObject> ();
	private List<GameObject> playerBullets = new List<GameObject> ();
	private List<GameObject> explosions = new List<GameObject> ();
	private List<GameObject> enemyBots = new List<GameObject> ();
	private List<GameObject> playerBots = new List<GameObject> ();


	public void ClearAll() {
		enemyBullets.Clear();
		playerBullets.Clear();
		explosions.Clear();
		enemyBots.Clear();
		playerBots.Clear();
	}


	public GameObject GetPlayerBullet (Vector3 initPosition)
	{
		return GetBullet (initPosition, true);
	}


	public GameObject GetEnemyBullet (Vector3 initPosition)
	{
		return GetBullet (initPosition, false);
	}


	private GameObject GetBullet (Vector3 initPosition, bool isPlayer)
	{
		List<GameObject> pool = enemyBullets;
		if (isPlayer) {
			pool = playerBullets;
		}

		foreach (GameObject go in pool) {
			if (go!=null && !go.activeInHierarchy) {
				go.transform.position = initPosition;
				go.transform.rotation = Quaternion.identity;
				go.SetActive (true);
				return go;
			}
		}

		GameObject bullet = MonoBehaviour.Instantiate (isPlayer ? playerBulletPrefab : enemyBulletPrefab, initPosition, Quaternion.identity);
		pool.Add (bullet);

		return bullet;
	}


	public GameObject GetExplosion (Vector3 initPosition)
	{
		foreach (GameObject go in explosions) {
			if (go!=null && !go.activeInHierarchy) {
				go.transform.position = initPosition;
				go.transform.rotation = Random.rotation;
				go.SetActive (true);
				return go;
			}
		}

		GameObject explosion = MonoBehaviour.Instantiate (explosionPrefab, initPosition, Random.rotation);
		explosions.Add (explosion);
		return explosion;
	}


	public GameObject GetPlayerBot(Vector3 initPosition) {
		return GetBot (initPosition, true);
	}


	public GameObject GetEnemyBot(Vector3 initPosition) {
		return GetBot (initPosition, false);
	}


	private GameObject GetBot (Vector3 initPosition, bool isPlayerTeam)
	{
		List<GameObject> pool = enemyBots;
		if (isPlayerTeam) {
			pool = playerBots;
		}

		foreach (GameObject go in pool) {
			if (go!=null && !go.activeInHierarchy) {
				go.transform.position = initPosition;
				go.transform.rotation = Quaternion.identity;
				go.SetActive (true);

				foreach (Transform child in go.transform) {
					child.gameObject.SetActive (true);
				}

				return go;
			}
		}

		GameObject bot = MonoBehaviour.Instantiate (isPlayerTeam ? playerBotPrefab : enemyBotPrefab, initPosition, Quaternion.identity);
		pool.Add (bot);

		return bot;
	}

}
