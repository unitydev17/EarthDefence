using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerData
{

	private static PlayerData instance;


	public static PlayerData Instance {
		get {
			if (instance == null) {
				instance = new PlayerData();
			}
			return instance;
		}
	}


	public int health;


	public void ResetHealth()
	{
		health = 100;
	}

}
