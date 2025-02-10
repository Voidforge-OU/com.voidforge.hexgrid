using Unity.Mathematics;
using UnityEngine;

public static class HexGrid
{
    public static int2 forward = new(1, 0);
    public static int2 backward = new(-1, 0);
    public static int2 left = new(0, 1);
    public static int2 right = new(1, -1);

    private const float h = 0.57735027f; // Mathf.Sqrt(3f) / 3f;

    private static Mesh _cellMesh;

    public static Mesh cellMesh {
        get {
            if (_cellMesh == null) _cellMesh = CreateCellMesh();
            return _cellMesh;
        }
    }

    public static float2 cellSize { get { return new float2(1f, 2f * h); } }

    public static Mesh CreateCellMesh()
    {
        return new()
        {
            name = "HexGrid Cell",
            vertices = new Vector3[] {
                new(0f, 0f, 0f),
                new(0f, h, 0f),
                new(0.5f, 0.5f * h, 0f),
                new(0.5f, -0.5f * h, 0f),
                new(0f, -h, 0f),
                new(-0.5f, -0.5f * h, 0f),
                new(-0.5f, 0.5f * h, 0f)
            },
            normals = new Vector3[] {
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.back
            },
            uv = new Vector2[] {
                new(0.5f, 1f),
                new(0f, 0f),
                new(1f, 0f),
                new(0f, 0f),
                new(1f, 0f),
                new(0f, 0f),
                new(1f, 0f)
            },
            triangles = new int[] {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 5,
                0, 5, 6,
                0, 6, 1
            },
            colors = new Color[] {
                Color.white,
                Color.cyan,
                Color.green,
                Color.yellow,
                Color.red,
                Color.magenta,
                Color.blue,
            }
        };
    }

    public static int2 GetCellIndex(float2 gridPosition) { return GetCellIndex(gridPosition, out float2 _); }

    public static int2 GetCellIndex(float2 gridPosition, out float2 offset) {

        offset = new float2(gridPosition.x * 2f, gridPosition.y * 2f / h); // scale y for ease of calculations
        int2 res = new(0, Mathf.RoundToInt(offset.y / 3f));
        res.x = Mathf.RoundToInt((offset.x - res.y) * 0.5f);
        offset.x -= 2 * res.x + res.y;
        offset.y -= 3 * res.y;
        // Acccount for the upper/lower parts d.y where hexes make the sawtooth pattern
        if (offset.y > 1f) {
            if (offset.y > 2f - offset.x) { 
                res.y++;
                offset.x -= 1f;
                offset.y -= 3f; 
            } else if (offset.y > 2f + offset.x) {
                res.x--;
                res.y++;
                offset.x += 1f;
                offset.y -= 3f; 
            }
        } else if (offset.y < -1f) {
            if (-offset.y > 2f - offset.x) {
                res.x++;
                res.y--;
                offset.x -= 1f;
                offset.y += 3f; 
            } else if (-offset.y > 2f + offset.x) {
                res.y--;
                offset.x += 1f;
                offset.y += 3f; 
            }
        }
        offset *= 0.5f;
        offset.y *= h; // Rememeber to un-scale y
        return res;
    }

    static public float2 GetCellPosition(int2 index) { return GetCellPosition(index.x, index.y); }

    static public float2 GetCellPosition(int i, int j) {
        return new float2(2 * i + j, 3 * j * h) * 0.5f;
    }
}