using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {

	private const int LAST_INTRO_NUM = 6;
	public static int introSceneNum = 1;

	public PlayableDirector director;

	void Update () {
		if (director.state != PlayState.Playing) {
			introSceneNum++;

			if (introSceneNum > LAST_INTRO_NUM) {
				LoadGameOrMenu ();
				return;
			}
			SceneManager.LoadScene ("Intro" + introSceneNum.ToString());
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Utils.SaveIntroActivated ();
			introSceneNum = 1;
			SceneManager.LoadScene ("Menu");
		}
	}


	void LoadGameOrMenu() {
		string nextScene;
		if (Utils.IsIntroStartedFromMenu ()) {
			Utils.SaveIntroActivated ();
			nextScene = "Menu";
		} else {
			nextScene = "Scene1";
			Utils.SaveIntroActivated ();
		}

		introSceneNum = 1;
		SceneManager.LoadScene (nextScene);
	}
}
