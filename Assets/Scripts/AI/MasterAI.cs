using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class MasterAI : MonoBehaviour
{

	[SerializeField]
	private int MAX_ITEMS_COUNT = 1;

	[SerializeField]
	private float SPAWN_TIME = 5f;

	public GameObject spawnPointObj;
	public GameObject defenceObj;
	public GameObject attackObj;

	private List<GameObject> items;


	void Awake()
	{
		items = new List<GameObject>();
	}


	void Start()
	{
		InvokeRepeating("SpawnItems", 0, SPAWN_TIME);
	}


	void SpawnItems()
	{
		if (items.Count >= MAX_ITEMS_COUNT) {
			return;
		}

		bool isPlayerTeam = attackObj != null; 


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