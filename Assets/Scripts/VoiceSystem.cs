using UnityEngine;
using Meta.WitAi.Dictation;
using Meta.WitAi.CallbackHandlers;
using UnityEngine.Events;
using Oculus.Voice;


public class VoiceSystem : MonoBehaviour
{
    // Current audio request for specific deactivation
    [SerializeField] private AppVoiceExperience _appVoiceExperience;
    [SerializeField] private WitResponseMatcher _witResponseMatcher;

    [SerializeField] private UnityEvent _wakeWordDetected;

    [SerializeField] private UnityEvent<string> _completeTranscription;

    private bool _voiceCommandReady;

    private string _transcriptionText;



    void Awake()
    {
        _appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(ReactivateVoice);
        _appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
        _appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscription);

        _appVoiceExperience.Activate();
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
    }

    private void OnDestroy()
    {
        _appVoiceExperience.VoiceEvents.OnRequestCompleted.RemoveListener(ReactivateVoice);
        _appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
        _appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnFullTranscription);
    }
}
