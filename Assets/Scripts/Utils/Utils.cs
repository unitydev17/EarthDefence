using UnityEngine;

public class Utils {

	private const int INTRO_WAS_ACTIVATED = 1;
	private const int INTRO_STARTED_FROM_MENU = 2;
	private const string INTRO_STATE = "intro_state";

	public static bool IsIntroActivated() {
		return PlayerPrefs.GetInt (INTRO_STATE) == 1;
	}

	public static bool IsIntroStartedFromMenu() {
		return PlayerPrefs.GetInt (INTRO_STATE) == 2;
	}

	public static bool IsFirstRun() {
		return PlayerPrefs.GetInt (INTRO_STATE) == 0;
	}

	public static bool IsNotFirstRun() {
		return PlayerPrefs.GetInt (INTRO_STATE) > 0;
	}

	public static void SaveIntroActivated() {
		PlayerPrefs.SetInt (INTRO_STATE, INTRO_WAS_ACTIVATED);
		PlayerPrefs.Save ();
	}

	public static void SaveIntroStartedFromMenu() {
		PlayerPrefs.SetInt (INTRO_STATE, INTRO_STARTED_FROM_MENU);
		PlayerPrefs.Save ();
	}
}
