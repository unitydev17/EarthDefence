using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void Credits()
    {
        SceneManager.LoadScene(5);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
