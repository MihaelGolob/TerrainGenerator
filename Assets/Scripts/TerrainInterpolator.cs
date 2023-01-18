using System;
using UnityEngine;

public enum InterpolationType { Linear, Subtract, Add}

public class TerrainInterpolator : GeneratedTerrain {
    // inspector assigned
    [SerializeField] private InterpolationType _interpolationType = InterpolationType.Linear;
    [SerializeField] [Range(0, 1)] private float _interpolationValue = 0.5f;
    [SerializeField] private GeneratedTerrain _terrain1;
    [SerializeField] private GeneratedTerrain _terrain2;

    // private
    private int _terrainGeneratedCount;

    private void Awake() {
        _terrainGeneratedCount = 0;
    }

    public override void GenerateTerrain() {
        _terrain1.GenerateTerrain();
        _terrain2.GenerateTerrain();
        InterpolateTerrain();
    }

    private void OnTerrainGeneratedHandler() {
        _terrainGeneratedCount++;
        if (_terrainGeneratedCount >= 2) {
            InterpolateTerrain();
        }
    }

    private void InterpolateTerrain() {
        // terrains must be the same size!
        if (_terrain1.Size.x != _terrain2.Size.x || _terrain1.Size.y != _terrain2.Size.y)
            throw new Exception($"Terrains must be the same size! (are: {_terrain1.Size} and {_terrain2.Size})");
        Size = _terrain1.Size;
        
        // initialize
        var numVertices = (_terrain1.Size.x + 1) * (_terrain1.Size.y + 1);
        _vertices = new Vector3[numVertices];
        _triangles = _terrain1.Terrain.triangles;

        switch (_interpolationType) {
            case InterpolationType.Linear:
                GenerateLinearTerrain();
                break;
            case InterpolationType.Subtract:
                GenerateTerrainSubtract();
                break;
            case InterpolationType.Add:
                GenerateTerrainAdd();
                break;
            default:
                throw new Exception("Interpolation type not defined!");
        }
        
        // InvokeOnTerrainGenerated();
    }

    private void GenerateLinearTerrain() {
        // linearly interpolate between the two terrains
        for (var y = 0; y <= _terrain1.Size.y; y++) {
            for (var x = 0; x <= _terrain1.Size.x; x++) {
                var pos = x + y * (_terrain1.Size.x + 1);
                var height = Mathf.Lerp(_terrain1.Terrain.vertices[pos].y, _terrain2.Terrain.vertices[pos].y, _interpolationValue);
                // set x and z to the same as the first terrain
                _vertices[pos] = _terrain1.Terrain.vertices[pos];
                // change vertex height
                _vertices[pos].y = height;
            }
        }
    }

    private void GenerateTerrainSubtract() {
        for (var y = 0; y <= _terrain1.Size.y; y++) {
            for (var x = 0; x <= _terrain1.Size.x; x++) {
                var pos = x + y * (_terrain1.Size.x + 1);
                var height = _terrain1.Terrain.vertices[pos].y - _terrain2.Terrain.vertices[pos].y;
                // set x and z to the same as the first terrain
                _vertices[pos] = _terrain1.Terrain.vertices[pos];
                // change vertex height
                _vertices[pos].y = height;
            }
        }
    }
    
    private void GenerateTerrainAdd() {
        for (var y = 0; y <= _terrain1.Size.y; y++) {
            for (var x = 0; x <= _terrain1.Size.x; x++) {
                var pos = x + y * (_terrain1.Size.x + 1);
                var height = _terrain1.Terrain.vertices[pos].y + _terrain2.Terrain.vertices[pos].y;
                // set x and z to the same as the first terrain
                _vertices[pos] = _terrain1.Terrain.vertices[pos];
                // change vertex height
                _vertices[pos].y = height;
            }
        }
    }
}