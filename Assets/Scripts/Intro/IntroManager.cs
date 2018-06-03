using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {

	public static int introSceneNum = 1;

	public PlayableDirector director;

	void Update () {
		if (director.state != PlayState.Playing) {
			introSceneNum++;
			SceneManager.LoadScene ("Intro" + introSceneNum.ToString());
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene ("Menu");
		}
	}
}
