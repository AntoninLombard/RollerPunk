using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;


[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(SplineContainer))]
[ExecuteInEditMode]
public class MeshBend : MonoBehaviour
{
    private Spline spline;
    private MeshCollider collider;
    private MeshFilter filter;
    
    
    [Serializable]
    public enum FillType
    {
        StretchAndRepeat,
        FillAsBest
    }

    [SerializeField] private FillType type;
    [SerializeField]private Mesh source;
    [SerializeField]private Vector3 scale;
    [SerializeField]private Vector3 translation;
    [SerializeField]private Vector3 rotation;
    [SerializeField]private int repetitions;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        spline = GetComponent<SplineContainer>().Splines.First();
        collider = GetComponent<MeshCollider>();
        filter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Refresh()
    {
        //TODO Update the mesh here
    }


    public void BuildMesh()
    {
        spline = GetComponent<SplineContainer>().Splines.First();
        collider = GetComponent<MeshCollider>();
        filter = GetComponent<MeshFilter>();
    
        
        spline = GetComponent<SplineContainer>().Splines.First();
        Mesh newMesh = new Mesh();
        float minX = source.vertices.Min(vec => vec.x);
        float maxX = source.vertices.Max(vec => vec.x);
        float meshLength = maxX - minX;
        float splineLength = spline.GetLength();
        float sectionLength = splineLength / repetitions;
        
        
        if (type == FillType.StretchAndRepeat)
            scale = Vector3.one *  sectionLength / meshLength;
        else if (type == FillType.FillAsBest)
        {
            repetitions = Mathf.RoundToInt(splineLength / (meshLength * scale.x)); // TODO Better rounding to closer int
            sectionLength = splineLength / repetitions;
            scale = Vector3.one * sectionLength;
        }

        
        // INITIALIZE THE NEW MESH
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector2> uv2 = new List<Vector2>();
        List<Vector2> uv3 = new List<Vector2>();
        List<Vector2> uv4 = new List<Vector2>();
        List<Vector2> uv5 = new List<Vector2>();
        List<Vector2> uv6 = new List<Vector2>();
        List<Vector2> uv7 = new List<Vector2>();
        List<Vector2> uv8 = new List<Vector2>();
        
        
        for (int i = 0; i < repetitions; i++) {
            foreach (var index in source.triangles) {
                triangles.Add(index + source.vertices.Length * i);
            }
            uv.AddRange(source.uv);
            uv2.AddRange(source.uv2);
            uv3.AddRange(source.uv3);
            uv4.AddRange(source.uv4);
#if UNITY_2018_2_OR_NEWER
            uv5.AddRange(source.uv5);
            uv6.AddRange(source.uv6);
            uv7.AddRange(source.uv7);
            uv8.AddRange(source.uv8);
#endif
        }
        
        
        //COMPUTE NEW VERTICES POSITIONS
        float offsetX = 0;
        for (int i = 0; i < repetitions; i++)
        {
            for(int j = 0; j < source.vertices.Length;j++)
            {
                Vector3 currentVert = source.vertices[j];
                Vector3 currentNormal = source.normals[j];
                float posX = (currentVert.x - minX)*scale.x + offsetX;
                
                currentVert = Vector3.Scale(currentVert,new Vector3(0,scale.y,scale.z));
                //currentVert = Quaternion.Euler(rotation) * currentVert;
                //currentVert += translation;
                if (!spline.Evaluate(posX / splineLength, out float3 splinePos, out float3 splineTangent,
                        out float3 splineUp)) continue;
                Quaternion splineRotation = Quaternion.LookRotation(splineTangent, splineUp) * Quaternion.Euler(0, -90, 0);

                Quaternion currentRot = splineRotation * Quaternion.Euler(rotation);


                currentVert.x = 0;
                currentVert = currentRot * currentVert;
                currentVert += (Vector3)splinePos + translation;
                normals.Add(splineRotation * currentNormal);
                vertices.Add(currentVert);
            }
            offsetX += sectionLength;
        }


        newMesh.vertices = vertices.ToArray();
        newMesh.normals = normals.ToArray();
        newMesh.uv = uv.ToArray();
        newMesh.uv2 = uv2.ToArray();
        newMesh.uv2 = uv3.ToArray();
        newMesh.uv4 = uv4.ToArray();
        newMesh.uv5 = uv5.ToArray();
        newMesh.uv6 = uv6.ToArray();
        newMesh.uv7 = uv7.ToArray();
        newMesh.uv8 = uv8.ToArray();
        newMesh.triangles = triangles.ToArray();


        newMesh.RecalculateBounds();
        newMesh.RecalculateTangents();


        newMesh.name = "Generated Bent Mesh";
        filter.sharedMesh = newMesh;
        collider.sharedMesh = newMesh;
    }



    //public void GetNewVertice(MeshVe)

}
