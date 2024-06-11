using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerInputData : ScriptableObject
{
    [SerializeField] public AnimationCurve driveCurve;
    [SerializeField] public AnimationCurve brakeCurve;
    [SerializeField] public AnimationCurve steerCurve;
    

}
