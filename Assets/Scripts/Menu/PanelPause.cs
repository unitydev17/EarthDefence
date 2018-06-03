using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PanelPause : MonoBehaviour
{

    public void Continue()
    {
        GameManager.Instance.IsPause = false;
        gameObject.SetActive(false);
		SoundController.instance.UnPauseBattleMusic ();
    }

    public void Exit()
    {
		GUIManager.UnsubscribeListeners ();
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

	public void Retry()
	{
		SceneManager.LoadScene ("Scene1");
	}

}

