using UnityEngine;

[CreateAssetMenu(fileName = "TrapSO", menuName = "Scriptable Objects/TrapSO")]
public class TrapSO : CardSO
{
    public enum StatType
    {
        Dexterity,
        Intelligence,
        Strength
    }

    public enum TargetType
    {
        Single,
        Party
    }

    [SerializeField] private float _damage;
    [SerializeField] private StatType _stat;
    [SerializeField] private int _difficulty;
    [SerializeField] private TargetType _target;

    public float Damage => _damage;
    public StatType Stat => _stat;
    public int Difficulty => _difficulty;
    public TargetType Target => _target;
}
