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
    [field :SerializeField] public VisualEffect parryVFX1 { get; private set; }
    [field :SerializeField] public VisualEffect parryVFX2 { get; private set; }
    [field :SerializeField] public VisualEffect counterVFX { get; private set; }
    
    
    
    private static readonly int PunchWindUpSpeed = Animator.StringToHash("PunchWindUpSpeed");
    private static readonly int PunchSpeed = Animator.StringToHash("PunchSpeed");
    private static readonly int ParrySpeed = Animator.StringToHash("ParrySpeed");
    private static readonly int TauntSpeed = Animator.StringToHash("TauntSpeed");
    
    void Start()
    {
        animator.SetFloat(PunchWindUpSpeed,data.punchWindUpR.length/player.data.punchWindUp);
        animator.SetFloat(PunchSpeed,data.punchL.length/player.data.punchDamageWindow);
        animator.SetFloat(ParrySpeed,data.parry.length/player.data.parryWindow);
        animator.SetFloat(TauntSpeed,data.parry.length/player.data.tauntDuration);
        punchLVFX.SetFloat("Lifetime",player.data.punchDamageWindow);
        punchRVFX.SetFloat("Lifetime",player.data.punchDamageWindow);
        shieldVFX.SetFloat("Lifetime",player.data.parryWindow);
            
    }


    public void SetColor(Color color)
    {
        punchLVFX.SetVector4("Main Color",color);
        punchRVFX.SetVector4("Main Color",color);
        parryVFX1.SetVector4("Main Color",color);
        parryVFX2.SetVector4("Main Color",color);
        counterVFX.SetVector4("Main Color",color);
        shieldVFX.SetVector4("Main Color",color);
    }
}
