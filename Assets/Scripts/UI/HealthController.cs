using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{

    public Image health;

	void Awake() {
		PlayerController.playerEvents += ProcessEvent;
	}


	void ProcessEvent(string command, object param) {
		if (GameController.HEALTH_UPDATE == command) {
			float value = (float)param;
			health.fillAmount = value / 100f;
		}
	}

}

