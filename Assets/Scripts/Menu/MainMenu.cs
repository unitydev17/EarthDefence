using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

	void OnEnable() {
		Cursor.visible = true;
	}


    public void Play()
    {
        SceneManager.LoadScene("Scene1");
    }


	public void Intro()
	{
		Utils.SaveIntroStartedFromMenu ();
		SceneManager.LoadScene("Intro");
	}

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

	public void Exit() {
		Utils.SaveIntroActivated ();
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}

}
