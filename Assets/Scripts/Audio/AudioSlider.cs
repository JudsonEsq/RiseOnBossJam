using UnityEngine;
using FMODUnity;
using UnityEngine.UI;
using FMOD.Studio;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private EventReference _sliderUpdateSound;
    [Tooltip("Path to the VCA that this slider will control. For example: 'vca:/SFX'")]
    [SerializeField] private string _vcaPath = "vca:/";
    private VCA _sliderVCA;
    private Slider _foundSlider;
    void Awake()
    {
        _foundSlider = GetComponent<Slider>();

        if(_foundSlider == null)
        {
            Debug.LogError($"[AudioSlider] '{gameObject.name}' requires a Slider component.");
        }

        if(string.IsNullOrEmpty(_vcaPath))
        {
            Debug.LogError($"[AudioSlider] '{gameObject.name}' requires a VCA path to control slider volume.");
        }
        else
        {
            _sliderVCA = RuntimeManager.GetVCA(_vcaPath);
            if(!_sliderVCA.isValid())
            {
                Debug.LogError($"[AudioSlider] '{gameObject.name}' has an invalid VCA path assigned: {_vcaPath}");
            }
        }
        
    }

    void OnEnable()
    {
        if(_foundSlider == null) return;
        _foundSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnDisable()
    {
        if(_foundSlider == null) return;
        _foundSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float newValue)
    {
        if(!_sliderUpdateSound.IsNull)
        {
           RuntimeManager.PlayOneShot(_sliderUpdateSound);
        }

        if(_sliderVCA.isValid())
        {
            _sliderVCA.setVolume(newValue);
        }
   
    }
}