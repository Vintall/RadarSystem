using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScutterRayMeshBuilder))]
public class ScutterRayMeshBuilderGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("RecalculateMesh"))
        {
            ((ScutterRayMeshBuilder)target).RecalculateMeshFromInspector();
        }
    }
}
