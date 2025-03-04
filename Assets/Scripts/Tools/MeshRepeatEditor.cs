using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MeshRepeat))]
public class MeshRepeatEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Update"))
        {
            MeshRepeat meshBend = (MeshRepeat)target;
            meshBend.Refresh();
        }
    }
}
#endif
