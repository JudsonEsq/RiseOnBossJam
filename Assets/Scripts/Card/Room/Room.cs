using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] private RoomSO roomSO;

    void Awake() {
        Debug.Log($"Room Cost: {roomSO.Cost}");
    }
}
