using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;


public class MasterAI : MonoBehaviour
{

	[SerializeField]
	private int MAX_ITEMS_COUNT = 50;

	[SerializeField]
	private float SPAWN_TIME = 5f;

	public GameObject spawnPointObj;
	public GameObject defenceObj;
	public GameObject attackObj;

	private List<GameObject> items;
	private int counter;

	private bool isPlayerTeam;
	private volatile bool isSpawnedAtLeastOne;

	public static event Action<string, object> masterAIEvents;


	public static void UnsubscribeAll() {
		Delegate[] clientList = masterAIEvents.GetInvocationList ();
		foreach (Delegate d in clientList) {
			masterAIEvents -= (d as Action<string, object>);
		}
	}


	void Awake()
	{
		items = new List<GameObject>();
	}


	void Start()
	{
		isSpawnedAtLeastOne = false;
		isPlayerTeam = attackObj != null; 
		counter = 0;
		InvokeRepeating("SpawnItems", 0, SPAWN_TIME);
	}


	void FixedUpdate() {
		if (!isPlayerTeam && isSpawnedAtLeastOne) {
			if (items.Count == 0) {
				isSpawnedAtLeastOne = false;
				masterAIEvents (GameController.GAME_WIN_EVENT, null);
			}
		}
	}


	void Update() {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			masterAIEvents (GameController.GAME_OVER_EVENT, null);
		}
	}

	void SpawnItems()
	{
		
		if (counter++ >= MAX_ITEMS_COUNT) {
			return;
		}

		isSpawnedAtLeastOne = true;

		GameObject item = isPlayerTeam ? Pools.Instance.GetPlayerBot (spawnPointObj.transform.position) : 
			Pools.Instance.GetEnemyBot (spawnPointObj.transform.position);

		item.transform.parent = GameController.root.transform;
		items.Add(item);

		var itemAI = item.GetComponent<ItemAI> ();
		itemAI.Init ();
		itemAI.SetMasterAI (this);

		if (isPlayerTeam) {
			itemAI.tag = CommonShipController.PLAYER_TEAM_TAG;
			itemAI.SetStrategy (new AttackStrategy (this, item.transform, attackObj.transform));
		} else {
			itemAI.tag = CommonShipController.ENEMY_TAG;
			itemAI.SetStrategy (new DefenceStrategy (this, item.transform, defenceObj.transform));
		}
	}


	public void RemoveEnemy(GameObject gameObject)
	{
		if (items.Count > 0) {
			items.Remove(gameObject);
		}
	}

}