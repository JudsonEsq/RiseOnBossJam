using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private TrapSO trapSO;

    void Awake() {
        Debug.Log($"Trap Cost: {trapSO.Cost}");
    }
}
