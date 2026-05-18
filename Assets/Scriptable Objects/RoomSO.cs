using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSO", menuName = "Scriptable Objects/RoomSO")]
public class RoomSO : CardSO
{
    public enum Layout
    {
        Up,
        Down,
        Left,
        Right
    }

    public List<Layout> doorLayouts;
}
