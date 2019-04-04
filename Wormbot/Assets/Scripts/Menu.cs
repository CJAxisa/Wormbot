using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public MovementScript wum;

    public SpriteRenderer[] MainMenuItems;
    public bool MainMenuFade;
    public SpriteRenderer[] InstructionItems;
    public float InstructionFade;
    public SpriteRenderer[] PauseMenuItems;

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MainMenuFade = true;
        }

        if (MainMenuFade)
        {
            foreach(SpriteRenderer spriteRenderer in MainMenuItems)
            {
                spriteRenderer.color -= new Color(0, 0, 0, .01f);
            }
        }

        if (MainMenuItems[0].color.a <= 1)
        {
            wum.enabled = true;
        }
    }
}
