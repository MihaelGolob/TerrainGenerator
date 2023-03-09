using System.Collections.Generic;
using UnityEngine;

public class VegetationSpawner : MonoBehaviour {
    [SerializeField] private List<GameObject> _vegetationPrefabs = new();
    [SerializeField] private int _spawnCount = 10;
    [SerializeField] private float _raycastHeight = 20f;

    [Header("Randomize")] 
    [SerializeField] private Vector2 _randomScale = new(0.5f, 1.5f);
    [SerializeField] private Vector2 _randomYRotation = new(0f, 360f);

    [Header("Constraints")]
    [SerializeField] private LayerMask _allowedLayers;
    [SerializeField] private Vector2 _spawnArea = new(10, 10);
    [SerializeField] private Vector2 _spawnHeight = new(2, 3);

    public void SpawnVegetation() {
        // remove all existing vegetation
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        var objectCounter = 0;
        var spawnPosition = transform.position;
        while (objectCounter < _spawnCount) {
            // Get random position
            var randomPosition = new Vector3(
                spawnPosition.x + Random.Range(0, _spawnArea.x) - _spawnArea.x / 2,
                0,
                spawnPosition.z + Random.Range(0, _spawnArea.y) - _spawnArea.y / 2);
            
            // raycast to get the ground position
            var ray = new Ray(randomPosition + Vector3.up * _raycastHeight , Vector2.down);
            if (Physics.Raycast(ray, out var hit, 100f, _allowedLayers)) {
                var pos = hit.point;
                if (pos.y >= _spawnHeight.x && pos.y <= _spawnHeight.y) {
                    var randomPrefab = _vegetationPrefabs[Random.Range(0, _vegetationPrefabs.Count)];
                    var go =Instantiate(randomPrefab, pos, Quaternion.identity, transform);
                    go.transform.localScale *= Random.Range(_randomScale.x, _randomScale.y);
                    go.transform.Rotate(0, Random.Range(_randomYRotation.x, _randomYRotation.y), 0);
                    
                    objectCounter++;
                }
            }
        }
    }
    
    public void ClearVegetation() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    public void OnDrawGizmos() {
        // draw spawn area
        Gizmos.color = Color.green;
        var pos = transform.position;
        Gizmos.DrawLine(new Vector3(pos.x - _spawnArea.x / 2, pos.y, pos.z - _spawnArea.y / 2), new Vector3(pos.x + _spawnArea.x / 2, pos.y, pos.z - _spawnArea.y / 2));
        Gizmos.DrawLine(new Vector3(pos.x + _spawnArea.x / 2, pos.y, pos.z - _spawnArea.y / 2), new Vector3(pos.x + _spawnArea.x / 2, pos.y, pos.z + _spawnArea.y / 2));
        Gizmos.DrawLine(new Vector3(pos.x + _spawnArea.x / 2, pos.y, pos.z + _spawnArea.y / 2), new Vector3(pos.x - _spawnArea.x / 2, pos.y, pos.z + _spawnArea.y / 2));
        Gizmos.DrawLine(new Vector3(pos.x - _spawnArea.x / 2, pos.y, pos.z + _spawnArea.y / 2), new Vector3(pos.x - _spawnArea.x / 2, pos.y, pos.z - _spawnArea.y / 2));
    }
}