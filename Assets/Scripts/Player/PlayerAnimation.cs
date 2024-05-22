using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] public PlayerAnimationData data;

    [field:SerializeField] public Animator animator { get; private set; }
    
    [field :Header("PARTICLES")]
    [field :SerializeField] public VisualEffect punchLVFX { get; private set; }
    [field :SerializeField] public VisualEffect punchRVFX { get; private set; }
    [field :SerializeField] public VisualEffect shieldVFX { get; private set; }
    [field :SerializeField] public VisualEffect parryLVFX { get; private set; }
    [field :SerializeField] public VisualEffect parryRVFX { get; private set; }
    [field :SerializeField] public VisualEffect counterVFX { get; private set; }
    
    
    void Start()
    {
        animator.SetFloat("PunchWindUpSpeed",data.punchWindUpR.length/player.data.punchWindUp);
        animator.SetFloat("PunchSpeed",data.punchL.length/player.data.punchDamageWindow);
        animator.SetFloat("ParrySpeed",data.parry.length/player.data.parryWindow);
        punchLVFX.SetFloat("Lifetime",player.data.punchDamageWindow);
        punchRVFX.SetFloat("Lifetime",player.data.punchDamageWindow);
        shieldVFX.SetFloat("Lifetime",player.data.parryWindow);
            
    }


    public void SetColor(Color color)
    {
        punchLVFX.SetVector4("Color",color);
        punchRVFX.SetVector4("Color",color);
        parryLVFX.SetVector4("Main Color",color);
        parryRVFX.SetVector4("Main Color",color);
        counterVFX.SetVector4("Color",color);
        shieldVFX.SetVector4("Color",color);
    }
}
