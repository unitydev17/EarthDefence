using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class AttackStrategy : DefenceStrategy
{

	public AttackStrategy(MonoBehaviour mono, Transform obj, Transform attackTarget) : base (mono, obj, attackTarget) {}

	protected override bool IsEnemy (string tag)
	{
		return tag == CommonShipController.ENEMY_TAG;
	}

}