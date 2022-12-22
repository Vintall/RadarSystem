using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RadarScutter))]
public class RadarScutterGUI : Editor
{
    string count = "";
    string distance = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("StartScuttering", GUILayout.Height(50)))
            ((RadarScutter)target).StartScuttering();

        if (GUILayout.Button("NextStep", GUILayout.Height(50)))
            ((RadarScutter)target).NextStep();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.TextArea("Antennas Count");
        
        count = GUILayout.TextField(count);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.TextArea("Distance between antennas");
        
        distance = GUILayout.TextField(distance);
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        if (GUILayout.Button("GenerateAntennas", GUILayout.Height(50)))
        {
            int int_count = -1;
            float distance_value;
            string distance_corrected = "";

            if (!string.IsNullOrEmpty(distance))
                for (int i = 0; i < distance.Length; i++)
                    if (distance[i] == '.') distance_corrected += ',';
                    else distance_corrected += distance[i];

            if (int.TryParse(count, out int_count))
                if (float.TryParse(distance_corrected, out distance_value))
                    ((RadarScutter)target).GenerateAntennas(int_count, distance_value);
        }

    }
}
