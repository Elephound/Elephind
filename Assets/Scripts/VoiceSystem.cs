using UnityEngine;
using Meta.WitAi.TTS.Utilities;
using Meta.WitAi.CallbackHandlers;
using UnityEngine.Events;
using Oculus.Voice;
using System.Collections;
using System;
using Oculus.Interaction.Samples;
using Unity.VisualScripting;

public class VoiceSystem : MonoBehaviour
{
    // Current audio request for specific deactivation
    [SerializeField] private AppVoiceExperience _appVoiceExperience;
    [SerializeField] private WitResponseMatcher _witResponseMatcher;

    [SerializeField] private UnityEvent _wakeWordDetected;

    [SerializeField] private UnityEvent<string> _completeTranscription;

    [SerializeField] private TTSSpeaker _speaker;

    
    private string _dateId = "[DATE]";



    private bool _voiceCommandReady;

    private string _transcriptionText;

    static public VoiceSystem Instance;

    [SerializeField] GameObject PlayerIcon;

    public UnityEvent<string> ShowTextOnUI;



    void Awake()
    {
        if(Instance == null)
            Instance = this;

        //_appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(ReactivateVoice);
        _appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
        _appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscription);

        StartCoroutine(ActivateDelayed());

        PlayerIcon.SetActive (false);
        SpeakText("Hi, i'm Elli. Ask me anything!");

    }


//do this to prevent an error 
    IEnumerator ActivateDelayed()
    {
        yield return null;
        _appVoiceExperience.Activate();
        yield return null;
        _appVoiceExperience.Deactivate();

        //_appVoiceExperience.Activate();
    }

    private void ReactivateVoice() => _appVoiceExperience.Activate();

    public void WakeWordDetected(string[] arg0)
    {
        _voiceCommandReady = true;
        _wakeWordDetected.Invoke();
    }


    void OnPartialTranscription(string transcription)
    {
        if (!_voiceCommandReady) return;
        _transcriptionText = transcription;
    }

    void OnFullTranscription(string transcription)
    {
        if (!_voiceCommandReady) return;

        _voiceCommandReady = false;
        _transcriptionText = transcription;
        _completeTranscription?.Invoke(_transcriptionText);
        PlayerIcon.SetActive(true);
    }

    private void OnDestroy()
    {
     //   _appVoiceExperience.VoiceEvents.OnRequestCompleted.RemoveListener(ReactivateVoice);
        _appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
        _appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnFullTranscription);
    }

    public void SpeakText(string text)
    {
        text = FormatText(text);
        ShowTextOnUI.Invoke(text);
      /*  if(_speaker.IsSpeaking)
                 StartCoroutine(SpeakAsync(text, queued));
        else */
        _speaker.Speak(text);

    }


   /*  private IEnumerator SpeakAsync(string phrase, bool queued)
        {
            // Queue
            if (queued)
            {
                yield return _speaker.SpeakQueuedAsync(new string[] { phrase });
            }
            // Default
            else
            {
                yield return _speaker.SpeakAsync(phrase);
            }

            // Play complete clip
            if (_asyncClip != null)
            {
                _speaker.AudioSource.PlayOneShot(_asyncClip);
            }
        } */


        // Format text with current datetime
        private string FormatText(string text)
        {
            string result = text;
            if (result.Contains(_dateId))
            {
                DateTime now = DateTime.UtcNow;
                string dateString = $"{now.ToLongDateString()} at {now.ToLongTimeString()}";
                result = text.Replace(_dateId, dateString);
            }
            return result;
        }



    

}
