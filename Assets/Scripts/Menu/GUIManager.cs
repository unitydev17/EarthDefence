using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Image health;

    public PanelPause panelPause;
    public PanelFinish panelFinish;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.IsPause = true;
            panelPause.gameObject.SetActive(true);
        }

        if (health.fillAmount <= 0)
        {
            panelFinish.gameObject.SetActive(true);
            Cursor.visible = true;
        }
    }

}

