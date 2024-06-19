using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] public PlayerAnimationData data;
    [SerializeField] public VfxManager vfx;

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
    private static readonly int Death1 = Animator.StringToHash("Death");
    private static readonly int Respawn1 = Animator.StringToHash("Respawn");
    private static readonly int VictoryDance = Animator.StringToHash("VictoryDance");


    private void OnEnable()
    {
        player.controller.boostReady += StartDriftBoostVFX;
        player.controller.driftCanceled += CancelDrift;
    }

    void Start()
    {
        animator.SetFloat(PunchWindUpSpeed,data.punchWindUpR.length/player.data.punchWindUp);
        animator.SetFloat(PunchSpeed,data.punchL.length/player.data.punchDamageWindow);
        animator.SetFloat(ParrySpeed,data.parry.length/player.data.parryWindow);
        animator.SetFloat(TauntSpeed,data.parry.length/player.data.tauntDuration);
        punchLVFX.SetFloat("Lifetime",player.data.punchDamageWindow);
        punchRVFX.SetFloat("Lifetime",player.data.punchDamageWindow);
        shieldVFX.SetFloat("Lifetime",player.data.parryWindow);
        vfx.punchWindUpL.SetFloat("Lifetime",player.data.punchWindUp);
        vfx.punchWindUpR.SetFloat("Lifetime",player.data.punchWindUp);
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

    
    
    
    public void StartDriftBoostVFX(int side)
    {
        vfx.onBoostReady(side);
    }
    
    public void CancelDrift(int side)
    {
        vfx.driftStop(side);
    }


    public void StartPunchWindUp(int side)
    {
        vfx.punchWindUpStart(side);
    }

    public void CancelPunchWindUp()
    {
        vfx.punchWindUpCancel();
    }

    public void Death()
    {
        vfx.death();
        animator.SetTrigger(Death1);
    }

    public void Respawn()
    {
        animator.SetTrigger(Respawn1);
    }

    public void StartVictoryPose()
    {
        animator.SetTrigger(VictoryDance);
    }
    
}
