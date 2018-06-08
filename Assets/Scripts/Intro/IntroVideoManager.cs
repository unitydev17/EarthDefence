using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoManager : MonoBehaviour
{

	public VideoPlayer vplayer;


	void Start ()
	{
		vplayer.Play ();
		vplayer.loopPointReached += ProcessVideoFinished;
	}


	void ProcessVideoFinished (VideoPlayer vp)
	{
		LoadGameOrMenu ();
	}


	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Utils.SaveIntroActivated ();
			SceneManager.LoadScene ("Menu");
		}
	}


	void LoadGameOrMenu ()
	{
		string nextScene;
		if (Utils.IsIntroStartedFromMenu ()) {
			Utils.SaveIntroActivated ();
			nextScene = "Menu";
		} else {
			nextScene = "Scene1";
			Utils.SaveIntroActivated ();
		}

		SceneManager.LoadScene (nextScene);
	}
}
