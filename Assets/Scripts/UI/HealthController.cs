using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{

    public Image health;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetDamage(health);
        }
    }

    public void GetDamage(Image healthLine)
    {
        StartCoroutine(Deleteline(healthLine));
    }
    public IEnumerator Deleteline(Image line)
    {
        int i = 0;
        while (i <= 10)
        {
            line.fillAmount -= 0.02f;
            i++;
            yield return new WaitForSeconds(0.03f);
        }
        yield return null;
    }
}

