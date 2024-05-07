using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseListener : MonoBehaviour
{
    public float[] VolumesOffset = new float[2]; //Panning offset
    AkChannelConfig cfg = new AkChannelConfig();
    private AK.Wwise.Switch playerNumber;

    void Start()
    {
        if (playerNumber.ToString() == "P1" || playerNumber.ToString() == "P3")
        {
            VolumesOffset[0] = 0;
            VolumesOffset[1] = -9;
        }

        if (playerNumber.ToString() == "P2" || playerNumber.ToString() =="P4")
        {
            VolumesOffset[0] = -9;
            VolumesOffset[1] = 0;
        }

        //Mofifying Listener spatialisation
        cfg.SetStandard(AkSoundEngine.AK_SPEAKER_SETUP_STEREO);
        AkSoundEngine.SetListenerSpatialization(this.gameObject, true, cfg, VolumesOffset);
    }
}