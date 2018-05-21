using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelFinish : MonoBehaviour {

    public void Exit()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
