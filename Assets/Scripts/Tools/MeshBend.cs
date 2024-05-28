using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;


[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(SplineContainer))]
[ExecuteInEditMode]
public class MeshBend : MonoBehaviour
{
    private bool needUpdate = true;
    private SplineContainer splineContainer;
    private Spline spline;
    private MeshCollider collider;
    private MeshFilter filter;
    
    
    [Serializable]
    public enum FillType
    {
        StretchAndRepeat,
        FillAsBest
    }
    [SerializeField]private bool liveUpdate;
    [SerializeField]private FillType type;
    [SerializeField]private Mesh source;
    [SerializeField]private Vector3 scale;
    [SerializeField]private Vector3 translation;
    [SerializeField]private Vector3 rotation;
    [SerializeField]private int repetitions;
    
    // Start is called before the first frame update
    void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        spline = splineContainer.Splines.First();
        collider = GetComponent<MeshCollider>();
        filter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (liveUpdate)
        {
            Refresh();
        }
    }
    
    public void Refresh()
    {
        //if (needUpdate)
            BuildMesh();
    }


    public int CheckIfDirty(int a)
    {
        return 1;
    }

    public void BuildMesh()
    {
        if(splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();
        if(collider == null)
            collider = GetComponent<MeshCollider>();
        if(filter == null)
            filter = GetComponent<MeshFilter>();
        if(collider == null)
            spline = GetComponent<SplineContainer>().Splines.First();

        spline = splineContainer.Spline;
        
        List<Mesh> newMeshes = new List<Mesh>();
        // for (int i = 0; i < source.subMeshCount; i++)
        // {
        //     subMeshes.Add(source.GetSubMesh(i));
        // }
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
        
        
        
        float offsetX = 0f;
        for (int rep = 0; rep < repetitions; rep++)
        {
            //Mesh newMesh = new Mesh();
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

            
            Mesh currentMesh = new Mesh();
            currentMesh.indexFormat = IndexFormat.UInt32;
            triangles.AddRange(source.triangles);
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
                
            for(int j = 0; j < source.vertexCount ;j++)
            {
                Vector3 currentVert = source.vertices[j];
                Vector3 currentNormal = source.normals[j];
                float posX = (currentVert.x - minX) * scale.x + offsetX;

                currentVert = Vector3.Scale(currentVert, new Vector3(0, scale.y, scale.z));
                //currentVert = Quaternion.Euler(rotation) * currentVert;
                //currentVert += translation;
                if (!spline.Evaluate(posX / splineLength, out float3 splinePos, out float3 splineTangent,out float3 splineUp)) continue;
                Quaternion splineRotation =
                    Quaternion.LookRotation(splineTangent, splineUp) * Quaternion.Euler(0, -90, 0);

                Quaternion currentRot = splineRotation * Quaternion.Euler(rotation);


                currentVert.x = 0;
                currentVert = currentRot * currentVert;
                currentVert += (Vector3)splinePos + translation;
                normals.Add(splineRotation * currentNormal);
                vertices.Add(currentVert);
            }
                
                
            currentMesh.vertices = vertices.ToArray();
            currentMesh.normals = normals.ToArray();
            currentMesh.uv = uv.ToArray();
            currentMesh.uv2 = uv2.ToArray();
            currentMesh.uv2 = uv3.ToArray();
            currentMesh.uv4 = uv4.ToArray();
            currentMesh.uv5 = uv5.ToArray();
            currentMesh.uv6 = uv6.ToArray();
            currentMesh.uv7 = uv7.ToArray();
            currentMesh.uv8 = uv8.ToArray();
            currentMesh.triangles = triangles.ToArray();
            currentMesh.hideFlags = source.hideFlags;
            #if UNITY_2017_3_OR_NEWER
            currentMesh.indexFormat = source.indexFormat;
            #endif

            currentMesh.RecalculateBounds();
            currentMesh.RecalculateTangents();
            List<SubMeshDescriptor> subMeshDescriptors = new List<SubMeshDescriptor>();
            for (int subMeshNb = 0; subMeshNb < source.subMeshCount; subMeshNb++)
            {
                subMeshDescriptors.Add(source.GetSubMesh(subMeshNb));
            }
            currentMesh.SetSubMeshes(subMeshDescriptors.ToArray());
            newMeshes.Add(currentMesh);
            offsetX += sectionLength;
        }

        
        Mesh newMesh = new Mesh();
        newMesh.indexFormat = IndexFormat.UInt32;
        newMesh.indexFormat = IndexFormat.UInt32;
        List<CombineInstance> finalCombineInstances = new List<CombineInstance>();
        
        for (int subMeshNb = 0; subMeshNb < source.subMeshCount; subMeshNb++)
        {
            List<CombineInstance> combineMeshes = new List<CombineInstance>();
            foreach (var mesh in newMeshes)
            {
                CombineInstance combineInstance = new CombineInstance
                {
                    mesh = mesh,
                    subMeshIndex = subMeshNb
                };
                combineMeshes.Add(combineInstance);
            }
            Mesh subMesh = new Mesh();
            subMesh.CombineMeshes(combineMeshes.ToArray(),true,false);
            CombineInstance finalCombineInstance = new CombineInstance
            {
                mesh = subMesh,
                subMeshIndex = 0
            };
            finalCombineInstances.Add(finalCombineInstance);
        }
        newMesh.CombineMeshes(finalCombineInstances.ToArray(),false,false);
        
        newMesh.RecalculateBounds();
        newMesh.RecalculateTangents();


        newMesh.name = "Generated Bent Mesh";
        filter.sharedMesh = newMesh;
        collider.sharedMesh = newMesh;
        
        
        

        
        // INITIALIZE THE NEW MESH
//         List<int> triangles = new List<int>();
//         List<Vector3> vertices = new List<Vector3>();
//         List<Vector3> normals = new List<Vector3>();
//         List<Vector2> uv = new List<Vector2>();
//         List<Vector2> uv2 = new List<Vector2>();
//         List<Vector2> uv3 = new List<Vector2>();
//         List<Vector2> uv4 = new List<Vector2>();
//         List<Vector2> uv5 = new List<Vector2>();
//         List<Vector2> uv6 = new List<Vector2>();
//         List<Vector2> uv7 = new List<Vector2>();
//         List<Vector2> uv8 = new List<Vector2>();
//         
//         
//         for (int i = 0; i < repetitions; i++) {
//             foreach (var index in source.triangles) {
//                 triangles.Add(index + source.vertices.Length * i);
//             }
//             uv.AddRange(source.uv);
//             uv2.AddRange(source.uv2);
//             uv3.AddRange(source.uv3);
//             uv4.AddRange(source.uv4);
// #if UNITY_2018_2_OR_NEWER
//             uv5.AddRange(source.uv5);
//             uv6.AddRange(source.uv6);
//             uv7.AddRange(source.uv7);
//             uv8.AddRange(source.uv8);
// #endif
//         }
//         
//         
//         //COMPUTE NEW VERTICES POSITIONS
//         int subMeshVertexStart = 0;
//         int subMeshIndexStart = 0;
//         // for (int subMeshNb = 0; subMeshNb < source.subMeshCount; subMeshNb++)
//         // {
//              float offsetX = 0;
//         //     SubMeshDescriptor currentSubMesh = source.GetSubMesh(subMeshNb);
//         //     SubMeshDescriptor newSubMesh = new SubMeshDescriptor();
//         //         
//         //     newSubMesh.baseVertex = currentSubMesh.baseVertex;
//         //     newSubMesh.topology = currentSubMesh.topology;
//         //     newSubMesh.firstVertex = subMeshVertexStart;
//         //     newSubMesh.indexStart = subMeshIndexStart;
//         //     newSubMesh.vertexCount = currentSubMesh.vertexCount * repetitions;
//         //     newSubMesh.indexCount = currentSubMesh.indexCount * repetitions;
//         //     
//         //     subMeshIndexStart = newSubMesh.indexStart;
//         //     subMeshVertexStart = newSubMesh.vertexCount;
//             for (int rep = 0; rep < repetitions; rep++)
//             {
//                 Mesh newMesh = new Mesh();
//                 for(int j = 0; j < source.vertexCount ;j++)
//                 {
//                     Vector3 currentVert = source.vertices[j];
//                     Vector3 currentNormal = source.normals[j];
//                     float posX = (currentVert.x - minX) * scale.x + offsetX;
//
//                     currentVert = Vector3.Scale(currentVert, new Vector3(0, scale.y, scale.z));
//                     //currentVert = Quaternion.Euler(rotation) * currentVert;
//                     //currentVert += translation;
//                     if (!spline.Evaluate(posX / splineLength, out float3 splinePos, out float3 splineTangent,
//                             out float3 splineUp)) continue;
//                     Quaternion splineRotation =
//                         Quaternion.LookRotation(splineTangent, splineUp) * Quaternion.Euler(0, -90, 0);
//
//                     Quaternion currentRot = splineRotation * Quaternion.Euler(rotation);
//
//
//                     currentVert.x = 0;
//                     currentVert = currentRot * currentVert;
//                     currentVert += (Vector3)splinePos + translation;
//                     normals.Add(splineRotation * currentNormal);
//                     vertices.Add(currentVert);
//                 }
//                 offsetX += sectionLength;
//             }
//             //subMeshes.Add(newSubMesh);
//         //}
//
//         // for (int i = 0; i < source.subMeshCount; i++)
//         // {
//         //     int indexStart = 0;
//         //     SubMeshDescriptor currentSubMesh = source.GetSubMesh(i);
//         //     for (int rep = 0; rep < repetitions; rep++)
//         //     {
//         //         for(int j = 0; j < currentSubMesh.indexCount;j++)
//         //         {
//         //             triangles.Add(rep * currentSubMesh.indexCount + j);
//         //         }
//         //     } 
//         // }
//         //newMesh.SetSubMesh();
//
//         newMesh.vertices = vertices.ToArray();
//         newMesh.normals = normals.ToArray();
//         newMesh.uv = uv.ToArray();
//         newMesh.uv2 = uv2.ToArray();
//         newMesh.uv2 = uv3.ToArray();
//         newMesh.uv4 = uv4.ToArray();
//         newMesh.uv5 = uv5.ToArray();
//         newMesh.uv6 = uv6.ToArray();
//         newMesh.uv7 = uv7.ToArray();
//         newMesh.uv8 = uv8.ToArray();
//         newMesh.triangles = triangles.ToArray();
//         newMesh.hideFlags = source.hideFlags;
// #if UNITY_2017_3_OR_NEWER
//         newMesh.indexFormat = source.indexFormat;
// #endif
//
//         //newMesh.SetSubMeshes(subMeshes);
//
//         newMesh.RecalculateBounds();
//         newMesh.RecalculateTangents();
//
//
//         newMesh.name = "Generated Bent Mesh";
//         filter.sharedMesh = newMesh;
//         //collider.sharedMesh = newMesh;
     }



    //public void GetNewVertice(MeshVe)

}
