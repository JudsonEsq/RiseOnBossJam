using UnityEngine;
using FMODUnity;
using static AudioEvents;
using System;
using System.Collections.Generic;

// Each row in the style sheet represents a different "style" of UI interaction (e.g. "Default", "SciFi", "Fantasy", etc.) and contains the FMOD events to play for each type of interaction (click, hover, slider update) for that style. When a UI element raises an interaction event, it will specify which style it wants to use, and the GlobalAudioManager will look up the appropriate sounds in this style sheet to play.
// it tells the stylesheet it's styleName is the tag of the button, and then the stylesheet looks up the appropriate sound for that tag and interaction type. This allows us to have multiple buttons with different tags all using the same style sheet, but getting different sounds based on their tag. If a button doesn't specify an override sheet, it will use the default sheet assigned to the GlobalAudioManager, and it will look up sounds based on its tag in that default sheet.

[Serializable] class AudioStyleSheetEntry
{
    public string styleName;
    public EventReference clickSound;
    public  EventReference hoverSound;
}

[CreateAssetMenu(fileName = "NewAudioStyleSheet", menuName = "Audio/Audio Style Sheet")]
public class AudioStyleSheet : ScriptableObject
{

    [SerializeField] private List<AudioStyleSheetEntry> _styleEntries = new List<AudioStyleSheetEntry>();

    public EventReference UIAudioInteractionEvent(string styleName, UIInteractionType interactionType)
    {
        foreach(var entry in _styleEntries)
        {
            if(entry.styleName == styleName)
            {
                switch (interactionType)
                {
                    case UIInteractionType.ButtonClick:
                        return entry.clickSound;
                    case UIInteractionType.ButtonHover:
                        return entry.hoverSound;
                    default:
                        return new EventReference();
                }
            }
        }
        Debug.LogWarning($"Style not found in Audio Style Sheet: {styleName}");
        return new EventReference();
    }

}