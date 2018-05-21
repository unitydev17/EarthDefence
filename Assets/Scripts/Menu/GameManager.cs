using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }

    }

    public event Action<bool> OnPause = delegate { };

    private bool isPause;

    private void Update()
    {
       
    }

    public bool IsPause
    {
        get
        {
            return isPause;
        }
        set
        {
            isPause = value;
            OnPause.Invoke(isPause);
            if (isPause == true)
            {
                Time.timeScale = 0;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}

