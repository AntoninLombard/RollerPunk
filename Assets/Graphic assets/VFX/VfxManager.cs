using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VfxManager : MonoBehaviour
{
    [SerializeField] public VisualEffect punchL, punchR, slashL, slashR, slashTornado01, slashTornado02, shield, DSparksHandL, DSparksHandR, DSparksWheelL, DSparksWheelR ;

    [SerializeField] public  VisualEffect deathArmL, deathArmR, deathHead, deathLeg, bloodBurst01, bloodBurst02, booster01, booster02, punchWindUpL, punchWindUpR ;
    public void playShield() {
        shield.Play() ;
    }
    
    public void playPunchL() {
        punchL.Play() ;
    }
    
    public void playPunchR() {
        punchR.Play() ;
    }
    
    public void playSlashL() {
        slashL.Play() ;
    }
    
    public void playSlashR() {
        slashR.Play() ;
    }

    public void ParrySuccess() {
        slashTornado01.Play() ;
        slashTornado02.Play() ;
    }

    public void driftStart(int direction) {
        if (direction == -1) {
            DSparksHandL.Play() ;
            DSparksWheelL.Play() ;
        } else {
            DSparksHandR.Play() ;
            DSparksWheelR.Play() ;
        }
    }

    public void onBoostReady(int direction) {
        if (direction == -1) {
            DSparksHandL.SendEvent("OnBoostReady") ;
            DSparksWheelL.SendEvent("OnBoostReady") ;
        } else {
            DSparksHandR.SendEvent("OnBoostReady") ;
            DSparksWheelR.SendEvent("OnBoostReady") ;
        }
    }
    public void driftStop(int direction) {
        if (direction == -1) {
            DSparksHandL.Stop() ;
            DSparksWheelL.Stop() ;
        } else {
            DSparksHandR.Stop() ;
            DSparksWheelR.Stop() ;
        }
        
    }

    public void stun() {
        bloodBurst02.Play() ;
    }

    public void death() {
        deathArmL.Play() ;
        deathArmR.Play() ;
        deathHead.Play() ;
        deathLeg.Play() ;
        bloodBurst01.Play() ;
        bloodBurst02.Play() ;
    }
    
    public void Boost() {
        booster01.Play() ;
        booster02.Play() ;
    }

    
    
    public void punchWindUpStart(int direction) {
        if (direction == -1) {
            punchWindUpL.Play();
        } else {
            punchWindUpR.Play();
        }
    }
    
    public void punchWindUpCancel() {
            punchWindUpL.Stop();
            punchWindUpR.Stop();
            punchWindUpL.enabled = false;
            punchWindUpR.enabled = false;
            punchWindUpL.enabled = true;
            punchWindUpR.enabled = true;
    }
}
