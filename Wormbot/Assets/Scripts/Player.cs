using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Player : MonoBehaviour
{
    public static int health = 0;


    // Start is called before the first frame update
    void Start()
    {
        health = 5;
        
    }

    public static void healthSubtract()
    {
        health--;

        if (health == 4)
        {
            GameObject.Find("heart 5").SetActive(false);
        }
        if (health == 3)
        {
            GameObject.Find("heart 4").SetActive(false);
        }

        if (health == 2)
        {
            GameObject.Find("heart 3").SetActive(false);
        }
        if (health == 1)
        {
            GameObject.Find("heart 2").SetActive(false);
        }
        if (health == 0)
        {
            GameObject.Find("heart 1").SetActive(false);
        }
    }

}
