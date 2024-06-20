using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class MiniMapBuilder : MonoBehaviour
{
    [SerializeField] private SplineContainer outputContainer;
    [SerializeField] private int resolution;
    [SerializeField] private List<SplineContainer> splineContainers;

    void OnEnable()
    {
        Spline fullSpline = new Spline();
        if (splineContainers.Count == 0) return;
        foreach (SplineContainer splineContainer in splineContainers)
        {
            foreach (BezierKnot knot in splineContainer.Spline.Knots)
            {
                BezierKnot currentKnot = knot;
                if (knot.Equals(splineContainer.Spline.Knots.First()))
                {
                    currentKnot.TangentIn = 0.1f * ((Vector3)currentKnot.TangentIn).normalized;
                } else if (knot.Equals(splineContainer.Spline.Knots.Last()))
                {
                    currentKnot.TangentOut = 0.1f * ((Vector3)currentKnot.TangentOut).normalized;
                }
                fullSpline.Add(currentKnot);
            }
        }

        fullSpline.Closed = true;
        outputContainer.Spline = fullSpline;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
