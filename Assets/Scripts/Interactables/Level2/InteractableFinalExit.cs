﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableFinalExit : Interactable
{
  
    public override void OnInteraction()
    {
       
            if (inventory.ContainsItem(220))
            {
                Level2Controller level2Controller = GameObject.Find("Level2Controller").GetComponent<Level2Controller>();
                StartCoroutine(level2Controller.LevelEnd());
            }
            else
            {
                MessageController.ShowMessage("The door is locked..And I don't think I'm done here\nyet.", Face.Thinking);

            }
        }
      
    
}