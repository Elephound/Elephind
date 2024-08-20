


using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;



public class HeadBoundUIManager : MonoBehaviour
{
    public UnityEvent welcomeScreenClosed,OnAppWasStarted,OnShowWelcomeScreen;
    
    void Start(){
    
        OnAppWasStarted.Invoke();
    
    }

   	[Button("close WelcomeScreen")]
    public void CloseWelcomeScreen(){

        welcomeScreenClosed.Invoke();

    }

    [Button("show WelcomeScreen")]
    public void ShowWelcomeScreen(){

        OnShowWelcomeScreen.Invoke();
    }
    
}
