using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBoxTrigger : MonoBehaviour
{
   [SerializeField] GameObject introBox;
    [SerializeField] GameObject chatBox;


   bool isShown = true;
   bool firstTime = true;

   public void AnyButton()
   {
        isShown = false;
        introBox.SetActive(false);
        if(firstTime)
        {
            firstTime = false;
            chatBox.SetActive(true);
        }
   }

   public void Reset()
   {
    isShown = true;
    introBox.SetActive(true);
   }
}
