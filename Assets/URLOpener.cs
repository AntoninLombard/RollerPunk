using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLOpener : MonoBehaviour
{
   public void URLOpen(string url)
    {
        Application.OpenURL(url);
    }
}
