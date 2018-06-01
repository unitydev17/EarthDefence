﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool isPause;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            IsPause = false;
        }
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
            if (isPause)
            {
                Time.timeScale = 0;
                Cursor.visible = true;
				CrossHairController.isEnabled = false;
            }
            else
            {
                Time.timeScale = 1;
				Cursor.visible = false;
				CrossHairController.isEnabled = true;
            }
        }
    }
}

