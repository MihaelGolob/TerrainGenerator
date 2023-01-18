using System;
using UnityEngine;

public abstract class GeneratedTerrain : MonoBehaviour {
    // protected
    protected Vector3[] _vertices;
    protected int[] _triangles;
    
    // public
    public (Vector3[] vertices, int[] triangles) Terrain => (_vertices, _triangles);
    public (int x, int y) Size;
    
    public abstract void GenerateTerrain();
}