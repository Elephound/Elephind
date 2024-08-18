using UnityEngine;
using Meta.WitAi;
using Meta.WitAi.Dictation;

public class VoiceSystem : MonoBehaviour
{
    /// <summary>
    /// Reference to the voice service that will be activated or deactivated
    /// </summary>
    [Tooltip("Reference to the current voice service")]
    [SerializeField] private VoiceService _voiceService;

      // Whether an audio request is still activated or not
        private bool _isActive = false;

    // Current audio request for specific deactivation
   
      [SerializeField] private DictationService _dictation;

        void Start()
        {

        }

        
        public void ToggleActivation()
        {
            if (_dictation.MicActive)
            {
                _dictation.Deactivate();
            }
            else
            {
                _dictation.Activate();
            }
        }
}
