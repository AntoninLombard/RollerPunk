using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VfxManager : MonoBehaviour
{
    [SerializeField] VisualEffect punchL, punchR, slashL, slashR, shield ;

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
}
