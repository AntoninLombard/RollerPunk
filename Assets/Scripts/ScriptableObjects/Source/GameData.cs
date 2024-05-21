using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    [Header("GAME VALUES")]
    [ColorUsage(true, true)]
    public List<Color> playerColors;
    
    
    [Header("SOUND")]
    public AK.Wwise.Event crowdSound;
}
