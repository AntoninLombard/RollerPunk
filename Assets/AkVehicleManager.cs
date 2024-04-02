using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class AkVehicleManager : MonoBehaviour
{
    public AK.Wwise.Event engineStart;
    public AK.Wwise.Event engineStop;

    // Start is called before the first frame update
    void Start()
    {
        engineStart.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
