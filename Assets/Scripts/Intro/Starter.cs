using UnityEngine;
using UnityEngine.SceneManagement;

public class Starter : MonoBehaviour {

	void Awake () {
		if (Utils.IsIntroActivated()) {
			SceneManager.LoadScene ("Menu");
			return;
		}
		Cursor.visible = false;
		SceneManager.LoadScene ("Intro1");
	}

}
