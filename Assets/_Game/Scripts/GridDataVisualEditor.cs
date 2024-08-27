using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridDataVisual))]
public class GridDataVisualEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridDataVisual script = (GridDataVisual)target;
        if (GUILayout.Button("Update Grid Data"))
        {
            script.UpdateGridDataWithNavMesh();
        }

        if (GUILayout.Button("Clear Grid Data"))
        {
            script._gridData.GetGridData.Clear();
        }

        if (GUILayout.Button("Save Grid Data"))
        {
            string path = EditorUtility.SaveFilePanel("Save Grid Data", Application.dataPath, "GridData", "asset");
            if (path.Length != 0)
            {
                string assetPath = path.Substring(path.IndexOf("Assets"));
                AssetDatabase.CreateAsset(script._gridData, assetPath);
                AssetDatabase.SaveAssets();
            }
        }


    }
}