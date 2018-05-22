using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class ItemAI : CommonShipController
{

	public const float FORCE = 0.2f;
	public const float BOUND_RADIUS = 5f;
	public const float ROTATION_SPEED = 1f;

	public const float VIEW_RADIUS = 200f;
	public const float ATTACK_RADIUS = 100f;
	public const float ATTACK_STOP_DISTANCE = 400f;

	public const float FIRE_FREQUENCY = 1f;

	//private Vector3 rightGunPosition = new Vector3(1.967f, 0.276f, 2f);
	//private Vector3 leftGunPosition = new Vector3(-1.967f, 0.276f, 2f);
	private Vector3 rightGunPosition = new Vector3(-0.87f, 0f, 11.6f);
	private Vector3 leftGunPosition = new Vector3(0.87f, 0f, 11.6f);


	private BaseStrategy strategy;
	private MasterAI masterAI;


	public void SetMasterAI(MasterAI masterAI) {
		this.masterAI = masterAI;
	}


	public void SetStrategy(BaseStrategy strategy)
	{
		this.strategy = strategy;
	}


	void Update()
	{
		strategy.Perform();

		//base.DebugPath(strategy.GetPath().GetWayPoints());
	}


	public void Fire() {
		base.Fire(leftGunPosition, rightGunPosition);
	}

	protected override void RemoveItemFromParent ()
	{
		masterAI.RemoveEnemy (gameObject);
	}
}