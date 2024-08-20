using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class HeadBoundUIManager : MonoBehaviour
{
    public UnityEvent welcomeScreenClosed,OnAppWasStarted;
    
    void Start(){
    
        OnAppWasStarted.Invoke();
    
    }

    public void CloseWelcomeScreen(){

        welcomeScreenClosed.Invoke();

    }
    
}
