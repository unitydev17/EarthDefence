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

    public void BackToMenu()
    {
		GUIManager.UnsubscribeListeners ();
        SceneManager.LoadScene("Menu");
		GameManager.Instance.IsPause = true;
    }

	public void Retry()
	{
		GUIManager.UnsubscribeListeners ();
		GameManager.Instance.IsPause = false;
		SceneManager.LoadScene ("Scene1");
	}

}

