using UnityEngine;
public static class AudioEvents
{
    #region Music

        public static event System.Action<int> OnMusicPriorityChanged;

        public static void RaiseMusicStateChanged(int newMusicPriority)
        {
            OnMusicPriorityChanged?.Invoke(newMusicPriority);
        }

    #endregion




    #region UI - Menu, Button Clicks, etc.
    public enum UIInteractionType
    {
        ButtonClick,
        ButtonHover
    }

        public static event System.Action<AudioTag, UIInteractionType> OnUIInteraction;

        public static void RaiseUIInteraction(AudioTag tag, UIInteractionType interactionType)
            {
                OnUIInteraction?.Invoke(tag, interactionType);
            }

    #endregion




    #region Gameplay - Player Actions 

        public static event System.Action OnRoomTilePlayed;
        public static void RaiseRoomTilePlayed() => OnRoomTilePlayed?.Invoke();
        public static event System.Action OnTrapTilePlayed;
        public static void RaiseTrapTilePlayed() => OnTrapTilePlayed?.Invoke();
        public static event System.Action OnTileDrawn;
        public static void RaiseTileDrawn() => OnTileDrawn?.Invoke();
        public static event System.Action OnTileDiscarded;
        public static void RaiseTileDiscarded() => OnTileDiscarded?.Invoke();
        public static event System.Action OnTileSelected;
        public static void RaiseTileSelected() => OnTileSelected?.Invoke();
        public static event System.Action OnTileDeselected;
        public static void RaiseTileDeselected() => OnTileDeselected?.Invoke();
        public static event System.Action OnTileHovered;
        public static void RaiseTileHovered() => OnTileHovered?.Invoke(); 
        public static event System.Action OnTileRotated;
        public static void RaiseTileRotated() => OnTileRotated?.Invoke();
        public static event System.Action OnTilePickedUp;
        public static void RaiseTilePickedUp() => OnTilePickedUp?.Invoke();

    #endregion



    #region Gameplay 

        public static event System.Action OnWarriorAttack;
        public static void RaiseWarriorAttack() => OnWarriorAttack?.Invoke();
        public static event System.Action OnWarriorDeath;
        public static void RaiseWarriorDeath() => OnWarriorDeath?.Invoke();
        public static event System.Action OnWarriorTakeDamage;
        public static void RaiseWarriorTakeDamage() => OnWarriorTakeDamage?.Invoke();
        public static event System.Action OnMageAttack;
        public static void RaiseMageAttack() => OnMageAttack?.Invoke();
        public static event System.Action OnMageTakeDamage;
        public static void RaiseMageTakeDamage() => OnMageTakeDamage?.Invoke();
        public static event System.Action OnMageDeath;
        public static void RaiseMageDeath() => OnMageDeath?.Invoke(); 
        public static event System.Action OnRogueAttack;
        public static void RaiseRogueAttack() => OnRogueAttack?.Invoke();
        public static event System.Action OnRogueTakeDamage;
        public static void RaiseRogueTakeDamage() => OnRogueTakeDamage?.Invoke();
        public static event System.Action OnRogueDeath;
        public static void RaiseRogueDeath() => OnRogueDeath?.Invoke();
        public static event System.Action OnAdventurerFindTreasure;
        public static void RaiseAdventurerFindTreasure() => OnAdventurerFindTreasure?.Invoke();

    #endregion


}