using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavMeshGenA))]
public class NavMeshGenAEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NavMeshGenA script = (NavMeshGenA)target;
        if (GUILayout.Button("Generate NavAreas"))
        {
            script.Build();
        }
    }
}