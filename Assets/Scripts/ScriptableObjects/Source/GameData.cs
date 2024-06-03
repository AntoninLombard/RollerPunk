using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    [Header("GAME VALUES")]
    public List<Color> playerColors;
    
    
    [Header("SOUND")]
    public AK.Wwise.Event countdownSound;
    public AK.Wwise.Event musicStart;
    public AK.Wwise.RTPC score;
    public AK.Wwise.Event crowdStart;
    public AK.Wwise.Event crowdScoring;
    public AK.Wwise.Event crowdKill;
    public AK.Wwise.Event crowdStun;
    public AK.Wwise.Event crowdSteal;
    public AK.Wwise.Event scoreUpSound;
    public AK.Wwise.Event crowdWaiting;
    public AK.Wwise.Event crowdRaceStart;
    public AK.Wwise.State[] musicState;
}
