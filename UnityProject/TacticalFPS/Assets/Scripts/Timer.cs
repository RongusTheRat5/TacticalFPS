using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    bool started = false;
    int count;
    private float timer;

    private void Start()
    {
        count = GameObject.FindGameObjectsWithTag("Target").Length;
    }

    private void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Target").Length != count)
        {
            if (!started) started = true;
            count = GameObject.FindGameObjectsWithTag("Target").Length;
            if (count == 0) started = false;
        }

        if (!started) return;
        timer += Time.deltaTime;
        GetComponent<TMPro.TMP_Text>().text = Mathf.Round(timer).ToString();
    }
}
