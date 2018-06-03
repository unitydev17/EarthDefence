using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GUIManager : MonoBehaviour
{

    public PanelPause panelPause;
	public PanelGameOver panelGameOver;
	public PanelGameWin panelGameWin;


	private void Awake() {
		GameManager.Instance.IsPause = false;

		PlayerController.playerEvents += ProcessEvents;
		MasterAI.masterAIEvents += ProcessEvents;
	}

	private void ProcessEvents(string command, object param)
	{
		if (GameController.GAME_OVER_EVENT == command) {
			StartCoroutine (DelayedGameOverPanel ());
		}

		if (GameController.GAME_WIN_EVENT == command) {
			StartCoroutine (DelayedStoryWin ());
			StartCoroutine (DelayedWinPanel ());
		}
	}


	IEnumerator DelayedStoryWin() {
		WaitForSeconds wait = new WaitForSeconds (GameController.GAME_WIN_DELAY / 2);
		yield return wait;
		SoundController.instance.PlayStoryWin ();
	}


	IEnumerator DelayedWinPanel() {
		WaitForSeconds wait = new WaitForSeconds (GameController.GAME_WIN_DELAY);
		yield return wait;
		GameManager.Instance.IsPause = true;
		panelGameWin.gameObject.SetActive (true);
	}


	IEnumerator DelayedGameOverPanel() {
		WaitForSeconds wait = new WaitForSeconds (GameController.GAME_OVER_DELAY);
		yield return wait;
		GameManager.Instance.IsPause = true;
		panelGameOver.gameObject.SetActive (true);
	}


    private void Update()
    {
		if (!GameManager.Instance.IsPause && Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.IsPause = true;
			SoundController.instance.PauseBattleMusic ();
            panelPause.gameObject.SetActive(true);
        }
    }


	public static void UnsubscribeListeners() {
		GameController.UnsubscribeAll ();
		PlayerController.UnsubscribeAll ();
		MasterAI.UnsubscribeAll ();
		Pools.Instance.ClearAll ();
	}
}

