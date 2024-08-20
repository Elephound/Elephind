using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBoxTrigger : MonoBehaviour
{
   [SerializeField] GameObject introBox;

   bool isShown = true;

   public void AnyButton()
   {
        isShown = false;
        introBox.SetActive(false);
   }

   public void Reset()
   {
    isShown = true;
    introBox.SetActive(true);
   }
}
