using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScoringData : ScriptableObject
{
    [Header("SCORING VALUES")] 
    public float distancePerPoint;
    public int killMultipliatorThreshold;
    public int killMultiplicatorValue;
    public int maxScore;
}
