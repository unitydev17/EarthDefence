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
				LoadMenu ();
				return;
			}
			SceneManager.LoadScene ("Intro" + introSceneNum.ToString());
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			LoadMenu ();
		}
	}


	void LoadMenu() {
		Utils.SaveIntroState ();
		SceneManager.LoadScene ("Menu");
	}
}
