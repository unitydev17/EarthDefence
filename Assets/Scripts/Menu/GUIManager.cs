using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GUIManager : MonoBehaviour
{

    public PanelPause panelPause;
	public PanelGameOver panelGameOver;


	private void Awake() {
		PlayerController.playerEvents += ProcessEvents;
	}

	private void ProcessEvents(string command)
	{
		if (GameController.GAME_OVER_EVENT == command) {
			panelGameOver.gameObject.SetActive (true);
		}
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.IsPause = true;
            panelPause.gameObject.SetActive(true);

        }
    }

}

