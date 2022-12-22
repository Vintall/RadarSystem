using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestRadar))]
public class TestRadarGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("CastRay"))
        {
            ((TestRadar)target).CastRay();
        }
    }
}
