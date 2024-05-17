using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player player;

    [field:SerializeField] public Animator animator { get; private set; }

    [Header("ANIMATIONS")]
    [SerializeField] private AnimationClip punchWindUpL;
    [SerializeField] private AnimationClip punchL;
    
    [field :Header("PARTICLES")]
    [field :SerializeField] public VisualEffect punchLVFX { get; private set; }
    [field :SerializeField] public VisualEffect punchRVFX { get; private set; }
    [field :SerializeField] public VisualEffect parryVFX { get; private set; }
    
    
    void Start()
    {
        animator.SetFloat("PunchWindUpSpeed",player.data.punchWindUp/punchWindUpL.length);
        animator.SetFloat("PunchSpeed",player.data.punchDamageWindow/punchL.length);
        punchLVFX.SetFloat("Lifetime",player.data.punchDamageWindow);
        punchRVFX.SetFloat("Lifetime",player.data.punchDamageWindow);
        parryVFX.SetFloat("Lifetime",player.data.parryWindow);
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
