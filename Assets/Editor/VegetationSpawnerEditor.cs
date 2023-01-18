using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VegetationSpawner))]
public class VegetationSpawnerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var spawner = (VegetationSpawner)target;
        if(GUILayout.Button("Spawn Vegetation")) {
            spawner.SpawnVegetation();
        }
    }
}

