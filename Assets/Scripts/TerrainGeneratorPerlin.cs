using UnityEngine;

public class TerrainGeneratorPerlin : GeneratedTerrain {
    // inspector assigned
    [SerializeField] private Vector2 _mapSize = new(20, 20);
    [SerializeField] private Vector2 _resolution = new(2, 2);

    [Header("Perlin noise")] 
    [SerializeField] [Range(0f, 20f)] private float _maxHeight = 2f;
    [SerializeField] private float _power = 1f;
    [SerializeField] [Range(0f, 1f)] private float _zoom = 0.2f;
    [SerializeField] private Vector2 _offset = new(2f, 4f);
    [SerializeField] [Range(0, 7)] private int _octaves = 3;
    
    [Header("Falloff")] 
    [SerializeField] private bool _useFalloff = true;
    [SerializeField] private FalloffType _falloffType = FalloffType.Circular;
    [SerializeField] private float _falloffStrength = 1f;
    [SerializeField] private float _falloffSpeed = 3f;
    [SerializeField] private float _falloffStart = 2.2f;
    [SerializeField] private Vector2 _falloffOffset = Vector2.zero;
    
    private float[,] _falloffMap;

    private void Awake() {
        // calculate number of triangles
        Size.x = (_mapSize.x * _resolution.x).ToInt();
        Size.y = (_mapSize.y * _resolution.y).ToInt();
    }

    private void Start() {
        CalculateShape();
    }
    
    public override void GenerateTerrain() {
        // calculate number of triangles
        Size.x = (_mapSize.x * _resolution.x).ToInt();
        Size.y = (_mapSize.y * _resolution.y).ToInt();
        
        CalculateShape();
    }

    private void CalculateShape() {
        var numVertices = (Size.x + 1) * (Size.y + 1);
        _vertices = new Vector3[numVertices];

        // create all vertices
        for (int i = 0, z = 0; z <= Size.y; z++) {
            for (var x = 0; x <= Size.x; x++, i++) {
                var perlinx = x * _zoom + _offset.x;
                var perliny = z * _zoom + _offset.y;
                var height = _maxHeight * Mathf.PerlinNoise(perlinx, perliny);
                height = Mathf.Pow(height, _power);

                var addHeight = 0f;
                // add octaves
                for (var j = 1; j <= _octaves; j++) {
                    var frequency = 2 * j * _zoom;
                    var amplitude = _maxHeight / (2 * j);
                    addHeight += amplitude * Mathf.PerlinNoise(frequency * x, frequency * z);
                }
                height += addHeight;
                
                _vertices[i] = new Vector3(x / _resolution.x, height, z / _resolution.y);
            }
        }

        // create all triangles
        var numTriangles = 2 * Size.x * Size.y;
        _triangles = new int[3 * numTriangles];

        for (int i = 0, z = 0; z < Size.y; z++) {
            for (var x = 0; x < Size.x; x++, i += 6) {
                var cur = z * (Size.x + 1) + x;
                // first triangle
                _triangles[i] = cur;
                _triangles[i + 1] = cur + (Size.x + 1);
                _triangles[i + 2] = cur + 1;

                // second triangle
                _triangles[i + 3] = cur + 1;
                _triangles[i + 4] = cur + (Size.x + 1);
                _triangles[i + 5] = cur + 1 + (Size.x + 1);
            }
        }
        
        // falloff
        if (_useFalloff) {
            _falloffMap = FalloffGenerator.GenerateFalloffMap(_falloffType, Size.x + 1,  _falloffSpeed, _falloffStart, _falloffOffset);
            ApplyFalloff();
        }
    }
    
    private void ApplyFalloff() {
        for (var y = 0; y <= Size.y; y++) {
            for (var x = 0; x <= Size.x; x++) {
                var pos = x + y * (Size.x + 1);
                var value = _falloffMap[y, x];
                Terrain.vertices[pos].y *= (1.0f - value) * _falloffStrength;
            }
        }
    }
}