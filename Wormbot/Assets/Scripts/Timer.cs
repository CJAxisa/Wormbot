using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{


    public float timeSoFar;
    public string displayTime;
    public static bool isCountingTime;

    // Start is called before the first frame update
    void Start()
    {
        timeSoFar = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCountingTime)
            timeSoFar += Time.deltaTime;


        displayTime = (timeSoFar / 60) + ":" + Mathf.Floor(timeSoFar%60)+"."+Mathf.Round((timeSoFar-Mathf.Floor(timeSoFar))*100)/100;

    }


    public void resetTimer()
    {
        timeSoFar = 0f;
    }
}
