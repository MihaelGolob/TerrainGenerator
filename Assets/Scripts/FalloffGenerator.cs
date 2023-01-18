using UnityEngine;

public enum FalloffType {Square, Circular}

public class FalloffGenerator {
    public static float[,] GenerateFalloffMap(FalloffType type, int size, float a, float b, Vector2 offset = default) => type switch {
        FalloffType.Square => GenerateSquareFalloffMap(size, a, b, offset),
        FalloffType.Circular => GenerateCircularFalloffMap(size, a, b, offset),
        _ => null
    }; 

    private static float[,] GenerateSquareFalloffMap(int size, float a, float b, Vector2 offset) {
        var map = new float[size, size];

        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                var x = (i / (float)(size - 1)) * 2 - 1;
                var y = (j / (float)(size - 1)) * 2 - 1;
                y -= offset.x;
                x -= offset.y;
                
                var value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = EvaluateGraph(value, a, b);
            }
        }

        return map; 
    }
    
    private static float [,] GenerateCircularFalloffMap(int size, float a, float b, Vector2 offset) {
        var map = new float[size, size];

        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                var x = (i / (float)(size - 1)) * 2 - 1;
                var y = (j / (float)(size - 1)) * 2 - 1;
                y -= offset.x;
                x -= offset.y;
                
                var value = Mathf.Sqrt(x * x + y * y);
                var clampedValue = Mathf.Clamp01(value);
                map[i, j] = EvaluateGraph(clampedValue, a, b);
            }
        }

        return map;
    }

    private static float EvaluateGraph(float x, float a, float b) {
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(b - b * x, a)); 
    }
}