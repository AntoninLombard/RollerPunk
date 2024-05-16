using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private Animator animator;

    [SerializeField] private AnimationClip punchWindUpL;
    [SerializeField] private AnimationClip punchL;
    
    
    void Start()
    {
        animator.SetFloat("PunchWindUpSpeed",player.data.punchWindUp/punchWindUpL.length);
        animator.SetFloat("PunchSpeed",player.data.punchDamageWindow/punchL.length);
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
