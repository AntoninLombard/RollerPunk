using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundSettings : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event fireSound;
    private List<GameObject> players = new List<GameObject>();

    void Start()
    {
        // Trouver tous les objets dans la scène
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Player(Clone)")
            {
                players.Add(obj);

                // Trouver l'enfant nommé "Camera"
                Transform childTransform = obj.transform.Find("Camera");

                if (childTransform != null)
                {
                    GameObject cameraGameObject = childTransform.gameObject;

                    // Enregistrer l'objet caméra enfant
                    AkSoundEngine.RegisterGameObj(cameraGameObject);

                    // Ajouter le listener
                    AkSoundEngine.AddListener(gameObject, cameraGameObject);

                    //Debug.Log($"Camera listener added for player: {obj.name}");

                    // Poster l'événement sonore
                    fireSound.Post(gameObject);
                }
                else
                {
                    //Debug.LogError($"Child 'Camera' not found under Player {obj.name}.");
                }
            }
        }

        if (players.Count == 0)
        {
            Debug.LogError("No players found in the scene.");
        }

        AkSoundEngine.RegisterGameObj(gameObject);
    }
}
