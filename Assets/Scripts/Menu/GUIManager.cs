using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public PanelPause panelPause;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.IsPause = true;
            panelPause.gameObject.SetActive(true);

        }
    }

}

