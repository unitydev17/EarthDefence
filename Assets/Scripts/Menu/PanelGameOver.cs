using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PanelGameOver : MonoBehaviour
{

	void OnEnable() {
		if (this.GetType() == typeof(PanelGameOver)) {
			SoundController.instance.PlayStoryRetry ();
		}
		Cursor.visible = true;
		CrossHairController.isEnabled = false;
	}

    public void Exit()
    {
		UnsubscribeListeners ();
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

	public void Retry()
	{
		UnsubscribeListeners ();
		Time.timeScale = 1;
		SceneManager.LoadScene ("Scene1");
	}

	public void UnsubscribeListeners() {
		GameController.UnsubscribeAll ();
		PlayerController.UnsubscribeAll ();
		MasterAI.UnsubscribeAll ();
		Pools.Instance.ClearAll ();
	}

}

