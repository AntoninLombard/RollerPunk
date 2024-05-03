using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventPlayer : MonoBehaviour
{
    public AK.Wwise.Event eventwW;
    // Start is called before the first frame update
    void Start()
    {
        eventwW.Post(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
