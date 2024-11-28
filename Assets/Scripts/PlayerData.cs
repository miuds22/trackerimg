using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    public int colorIndex;
    public int hiltIndex;
    public int highScore;

    [ColorUsage(true, true)]
    public Color colorValue;
}
