using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainRenderer : MonoBehaviour {
    [SerializeField] private GeneratedTerrain _terrain;
    [Header("Terrain coloring")]
    [SerializeField] private Material _terrainMaterial;
    [SerializeField] private Gradient _terrainGradient;
    [SerializeField] private Vector2 _height = new (1, 4);
    [SerializeField] private float _yOffset = -2f;
    
    
    [Header("Falloff")] 
    [SerializeField] private bool _useFalloff = true;
    [SerializeField] private FalloffType _falloffType = FalloffType.Square;
    [SerializeField] private float _falloffStrength = 1f;
    [SerializeField] private float _falloffSpeed = 3f;
    [SerializeField] private float _falloffStart = 2.2f;
    [SerializeField] private Vector2 _falloffOffset = Vector2.zero;

    // private variables
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private MeshCollider _collider;
    private float[,] _falloffMap;

    private void Awake() {
        CreateTerrain();
    }
    
    private void CreateTerrain() {
        _falloffMap = FalloffGenerator.GenerateFalloffMap(_falloffType, _terrain.Size.x + 1,  _falloffSpeed, _falloffStart, _falloffOffset);

        if (_useFalloff) 
            ApplyFalloff();
        
        ApplyToMesh();
    }

    private void ColorTerrain(out Color[] vertexColors) {
        vertexColors = new Color[_terrain.Terrain.vertices.Length];
        for (var i = 0; i < vertexColors.Length; i++) {
            var height = _terrain.Terrain.vertices[i].y + _yOffset;
            var normalizedHeight = Mathf.InverseLerp(_height.x, _height.y, height);
            vertexColors[i] = _terrainGradient.Evaluate(normalizedHeight);
        } 
    }

    private void ApplyToMesh() {
        ColorTerrain(out var vertexColors);
        
        var mesh = new Mesh {
            // configure mesh
            vertices = _terrain.Terrain.vertices,
            triangles = _terrain.Terrain.triangles,
            colors = vertexColors
        };
        
        mesh.RecalculateNormals();
        // set the mesh to the mesh filter
        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.sharedMesh = mesh;
        
        if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.sharedMaterial = _terrainMaterial;
        
        if (_collider == null) _collider = GetComponent<MeshCollider>();
        _collider.sharedMesh = mesh;
    }

    private void ApplyFalloff() {
        for (var y = 0; y <= _terrain.Size.y; y++) {
            for (var x = 0; x <= _terrain.Size.x; x++) {
                var pos = x + y * (_terrain.Size.x + 1);
                var value = _falloffMap[y, x];
                _terrain.Terrain.vertices[pos].y *= (1.0f - value) * _falloffStrength;
            }
        }
    }

    public void RenderTerrain() {
        _terrain.GenerateTerrain();
        
        if (_useFalloff) {
            _falloffMap = FalloffGenerator.GenerateFalloffMap(_falloffType, _terrain.Size.x + 1,  _falloffSpeed, _falloffStart, _falloffOffset);
            ApplyFalloff();
        }
        ApplyToMesh();
    }
}