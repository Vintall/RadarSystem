using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BayraktarCamFolower))]
public class BayraktarCamFolowerInspectorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Bake Initial Position"))
            ((BayraktarCamFolower)target).BakeInitialPos();
    }
}
