using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerAnimationData : ScriptableObject
{
    [SerializeField] public AnimationClip punchWindUpR;
    [SerializeField] public AnimationClip punchWindUpL;
    [SerializeField] public AnimationClip punchL;
    [SerializeField] public AnimationClip punchR;
    [SerializeField] public AnimationClip counterL;
    [SerializeField] public AnimationClip counterR;
    [SerializeField] public AnimationClip parry;
    [SerializeField] public AnimationClip parrySucessL;
    [SerializeField] public AnimationClip parrySucessR;
    [SerializeField] public AnimationClip tauntL;
    [SerializeField] public AnimationClip tauntR;

}
