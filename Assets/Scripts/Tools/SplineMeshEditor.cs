using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshBend))]
public class SplineMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Update"))
        {
            MeshBend meshBend = (MeshBend)target;
            meshBend.Refresh();
        }
    }
}
