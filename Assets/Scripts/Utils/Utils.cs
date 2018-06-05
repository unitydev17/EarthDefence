using UnityEngine;

public class Utils {

	private const int INTRO_WAS_ACTIVATED = 1;
	private const string INTRO_STATE = "intro_state";

	public static bool IsIntroActivated() {
		return PlayerPrefs.GetInt (INTRO_STATE) > 0;
	}

	public static void SaveIntroState() {
		PlayerPrefs.SetInt (INTRO_STATE, INTRO_WAS_ACTIVATED);
	}
}
