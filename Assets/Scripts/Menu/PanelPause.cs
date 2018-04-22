using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PanelPause : MonoBehaviour
{

    public void Continue()
    {
        GameManager.Instance.IsPause = false;
        gameObject.SetActive(false);
        Cursor.visible = false;
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

}

