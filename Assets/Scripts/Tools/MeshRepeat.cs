using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;


[DisallowMultipleComponent]
[ExecuteInEditMode]
public class MeshRepeat : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private Spline spline;
    [Serializable]
    struct Repeat
    {
        public GameObject gameObject;
        public float offset;
        public Vector3 rotation;
        public Vector3 scale;
        public int number;
    }


    [SerializeField] private List<Repeat> repeats;

    public void Refresh()
    {
        ClearChildrens();
        spline = splineContainer.Spline;
        float splineLength = spline.GetLength();
        foreach (var repeat in repeats)
        {
            float splineStep = 1f/repeat.number + repeat.offset/splineLength;
            for(int i = 0; i < repeat.number ; i++)
            {
                float3 position, tangent, upVector;
                spline.Evaluate((splineStep * i) % 1, out position, out tangent,
                    out upVector);
                GameObject child = Instantiate(repeat.gameObject);
                child.transform.parent = transform;
                child.transform.position = (Vector3)position + splineContainer.gameObject.transform.position;
                child.transform.rotation = Quaternion.LookRotation(tangent, upVector) * Quaternion.Euler(repeat.rotation);
                child.transform.localScale = repeat.scale;
            }
        }
    }
    
    public void ClearChildrens()
    {
        while(transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
#if UNITY_EDITOR
            DestroyImmediate(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }
    }
}
