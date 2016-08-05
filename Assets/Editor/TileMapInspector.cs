using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TileMap))]
public class TileMapInspector : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Regenerate"))
        {
            var tileMap = (TileMap)target;
            tileMap.BuildMesh();
            tileMap.BuildTexture();
        }
    }
}
