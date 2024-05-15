using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseListener : MonoBehaviour
{
    private float[] VolumesOffset = new float[2]; //Panning offset
    AkChannelConfig cfg = new AkChannelConfig();

   public void SetVolumeOffset(int nb)
    {
        if (nb%2 == 1)
        {
            VolumesOffset[0] = 0f;
            VolumesOffset[1] = -9f;
        }
        else if (nb%2 == 0)
        {
            VolumesOffset[0] = -9f;
            VolumesOffset[1] = 0f;
        }

        //Mofifying Listener spatialisation
        cfg.SetStandard(AkSoundEngine.AK_SPEAKER_SETUP_STEREO);
        AkSoundEngine.SetListenerSpatialization(this.gameObject, true, cfg, VolumesOffset);
    }

}