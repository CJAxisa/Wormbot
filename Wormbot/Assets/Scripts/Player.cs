using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log(health);
    }

}
