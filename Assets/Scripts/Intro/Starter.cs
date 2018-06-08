using UnityEngine;
using UnityEngine.SceneManagement;

public class Starter : MonoBehaviour {

	void Awake () {
		if (Utils.IsIntroActivated()) {
			SceneManager.LoadScene ("Menu");
			return;
		}
		Cursor.visible = false;

		/* Use this for animation intro */
		//SceneManager.LoadScene ("Intro1");

		/* Use this for video intro */
		SceneManager.LoadScene ("IntroVideo");
	}

}
