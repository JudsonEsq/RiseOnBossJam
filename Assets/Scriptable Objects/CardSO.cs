using UnityEngine;

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    [SerializeField] protected int _cost;
    [SerializeField] protected Sprite thumbnailSprite;
    [SerializeField] protected Sprite descriptionSprite;

    public int Cost => _cost;
}
