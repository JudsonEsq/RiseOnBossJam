using UnityEngine;
using FMODUnity;
using static AudioEvents;

public class GlobalAudioManager : MonoBehaviour
{
    [Tooltip("Default audio style sheet for UI interactions. Individual buttons can override this with their own style sheet if they want unique sounds.")]
    [SerializeField] private AudioStyleSheet _audioStyleSheet; //fallback when a button has not override sheet assigned, or for non-UI sounds that don't have a specific style sheet

    #region FMOD Events
        [Header("Music")]
        [SerializeField] private EventReference _musicEvent;
        [SerializeField] private string _musicParameterName = "MusicState"; //name of the parameter in FMOD that controls music state changes
        private FMOD.Studio.EventInstance musicInstance; //live playing instance of music, so we can control it (pause, stop, etc.) when the music state changes

        [Space(10)]
        [Header("Tile Sounds")]
        [SerializeField] private EventReference roomTilePlayedEvent;
        [SerializeField] private EventReference trapTilePlayedEvent;
        [SerializeField] private EventReference tileDrawnEvent;
        [SerializeField] private EventReference tileDiscardedEvent;
        [SerializeField] private EventReference tileSelectedEvent;
        [SerializeField] private EventReference tileDeselectedEvent;
        [SerializeField] private EventReference tileHoveredEvent;
        [SerializeField] private EventReference tileRotatedEvent;
        [SerializeField] private EventReference tilePickedUpEvent;
        [SerializeField] private EventReference tileReleasedEvent;
        [SerializeField] private EventReference organizeInventoryEvent;


        [Space(10)]
        [Header("Character Actions")]
        [SerializeField] private EventReference warriorAttackEvent;
        [SerializeField] private EventReference warriorDeathEvent;
        [SerializeField] private EventReference warriorTakeDamageEvent;
        [SerializeField] private EventReference mageAttackEvent;
        [SerializeField] private EventReference mageTakeDamageEvent;
        [SerializeField] private EventReference mageDeathEvent;
        [SerializeField] private EventReference rogueAttackEvent;
        [SerializeField] private EventReference rogueTakeDamageEvent;
        [SerializeField] private EventReference rogueDeathEvent;
        [SerializeField] private EventReference adventurerFindTreasureEvent;


    #endregion


    #region Singleton
    public static GlobalAudioManager Instance {get; private set;}

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    #endregion


        // Subscriptions
        OnUIInteraction += HandleUIInteraction;


        OnMusicPriorityChanged += HandleMusicStateChanged;

        OnRoomTilePlayed += () => Play2DOneShot(roomTilePlayedEvent);
        OnTrapTilePlayed += () => Play2DOneShot(trapTilePlayedEvent);
        OnTileDrawn += () => Play2DOneShot(tileDrawnEvent);
        OnTileDiscarded += () => Play2DOneShot(tileDiscardedEvent);
        OnTileSelected += () => Play2DOneShot(tileSelectedEvent);
        OnTileDeselected += () => Play2DOneShot(tileDeselectedEvent);
        OnTileHovered += () => Play2DOneShot(tileHoveredEvent);
        OnTileRotated += () => Play2DOneShot(tileRotatedEvent);
        OnTilePickedUp += () => Play2DOneShot(tilePickedUpEvent);
        OnTileReleased += () => Play2DOneShot(tileReleasedEvent);
        OnOrganizeInventory += () => Play2DOneShot(organizeInventoryEvent);


        OnWarriorAttack += () => Play2DOneShot(warriorAttackEvent);
        OnWarriorDeath += () => Play2DOneShot(warriorDeathEvent);
        OnWarriorTakeDamage += () => Play2DOneShot(warriorTakeDamageEvent);
        OnMageAttack += () => Play2DOneShot(mageAttackEvent);
        OnMageTakeDamage += () => Play2DOneShot(mageTakeDamageEvent);
        OnMageDeath += () => Play2DOneShot(mageDeathEvent);
        OnRogueAttack += () => Play2DOneShot(rogueAttackEvent);
        OnRogueTakeDamage += () => Play2DOneShot(rogueTakeDamageEvent);
        OnRogueDeath += () => Play2DOneShot(rogueDeathEvent);
        OnAdventurerFindTreasure += () => Play2DOneShot(adventurerFindTreasureEvent); 

    }

    void HandleUIInteraction(AudioTag tag, UIInteractionType interactionType)
    {
        AudioStyleSheet sheetToUse = tag.OverrideSheet != null ? tag.OverrideSheet : _audioStyleSheet;
        
        EventReference eventToPlay = sheetToUse.UIAudioInteractionEvent(tag.Tag, interactionType);
        if (eventToPlay.IsNull)
        {
            Debug.LogWarning($"No audio event assigned for {interactionType} in the Audio Style Sheet: {sheetToUse.name}");
            return;
        }
        RuntimeManager.PlayOneShot(eventToPlay);
    }

    void HandleMusicStateChanged(int newMusicPriority)
    {
        if(newMusicPriority == -1)
        {
            if(musicInstance.isValid())
            {
                musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                musicInstance.release();
            }

            return;
        }
        else if(musicInstance.isValid())
        {
            musicInstance.setParameterByName(_musicParameterName, newMusicPriority);
            return;
        }

        // If we reach this point, it means music was not already playing, so we need to start it and set the parameter
        musicInstance = RuntimeManager.CreateInstance(_musicEvent);
        musicInstance.setParameterByName(_musicParameterName, newMusicPriority);
        musicInstance.start();
    }


    void OnDestroy()
    {

        OnUIInteraction -= HandleUIInteraction;

        OnMusicPriorityChanged -= HandleMusicStateChanged;

        if(musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }



    }

    private void Play2DOneShot(EventReference eventRef)
    {
        if(eventRef.IsNull)
        {
            Debug.LogWarning("Attempted to play a null audio event reference.");
            return;
        }
        FMODUnity.RuntimeManager.PlayOneShot(eventRef);
    }

}
