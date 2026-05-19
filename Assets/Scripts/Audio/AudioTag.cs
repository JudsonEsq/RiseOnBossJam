using UnityEngine;
using UnityEngine.EventSystems;
using static AudioEvents;


// This component is added to any UI element that wants to trigger audio events when interacted with. It allows you to specify a "tag" for the element, which is used by the AudioStyleSheet to determine which sounds to play for different interactions (click, hover, etc.). 
// It also allows you to specify an override AudioStyleSheet if you want this element to use a different set of sounds than the default one assigned to the GlobalAudioManager. 
//When the element is clicked or hovered over, it raises a UI interaction event with its tag and the type of interaction, which the GlobalAudioManager listens for and responds to by playing the appropriate sound from the style sheet.

public class AudioTag : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private AudioStyleSheet _overrideSheet;

    public AudioStyleSheet OverrideSheet => _overrideSheet;

    [SerializeField] private string _tag; 
    public string Tag => _tag;

    public void OnPointerClick(PointerEventData eventData)
    {
        RaiseUIInteraction(this, UIInteractionType.ButtonClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RaiseUIInteraction(this, UIInteractionType.ButtonHover);
    }

}