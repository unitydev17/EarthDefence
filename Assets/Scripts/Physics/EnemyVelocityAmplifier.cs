using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyVelocityAmplifier : MonoBehaviour
{

	protected static GameObject[] planets;

	[SerializeField]
	private float SLOWAGE_DISTANCE = 1000f;


	Rigidbody rigidbodyCached;
	GameObject target;


	void Awake()
	{
		rigidbodyCached = GetComponent<Rigidbody>();
	}


	void Start()
	{
		target = GetComponent<EnemyAIOld>().GetPlayer();
	}


	void Update()
	{
		CheckTargetDistance();
	}


	void CheckTargetDistance()
	{
		float dist = Vector3.Distance(transform.position, target.transform.position);

		if (dist < SLOWAGE_DISTANCE) {

			float multiplier = (dist - EnemyAIOld.MIN_ATTACK_DISTANCE) / (SLOWAGE_DISTANCE - EnemyAIOld.MIN_ATTACK_DISTANCE);
			float maxVelocity = CommonShipController.MAX_VELOCITY * multiplier;
					
			if (rigidbodyCached.velocity.magnitude >= maxVelocity) {
				rigidbodyCached.velocity = maxVelocity * rigidbodyCached.velocity.normalized;
			}

		}
	}

}
