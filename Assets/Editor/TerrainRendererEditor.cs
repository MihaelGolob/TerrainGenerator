using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainRenderer))]
public class TerrainRendererEditor : Editor {
    public override void OnInspectorGUI() {
        var terrainRenderer = (TerrainRenderer)target;
        
        if (DrawDefaultInspector())
            terrainRenderer.RenderTerrain();
        
        if (GUILayout.Button("Generate Terrain")) {
            terrainRenderer.RenderTerrain();
        }
    } 
}
