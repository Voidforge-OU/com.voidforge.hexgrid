using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class HexGridObject : MonoBehaviour
{
    public int width = 5;
    public int height = 3;

    public GameObject cellPrefab;

    public Material hexMaterial;

    public Transform marker;

    private bool _isPopulated = false;

    public GameObject CreateCell() {
        if (cellPrefab != null) return Instantiate<GameObject>(cellPrefab);
        return CreateHexCellObject(hexMaterial);
    }

    static public GameObject CreateHexCellObject(Material material, Mesh hexMesh = null) {
        GameObject go = new GameObject("cell");
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = hexMesh ?? HexGrid.cellMesh;
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        return go;
    }

    public float2 GetPosition(Ray ray) {
        float3 origin = new(this.transform.InverseTransformPoint(ray.origin));
        float3 direction = new(this.transform.InverseTransformDirection(ray.direction));
        if (direction.z * math.sign(origin.z) >= 0f) return new(float.NaN, float.NaN);
        float scale = -origin.z / direction.z;
        return new(
            direction.x * scale + origin.x, 
            direction.y * scale + origin.y
        );
    }

    private void PopulateGrid() {
        int jmin = -(height >> 1);
        int jmax = height + jmin;
        for (int j = jmin; j < jmax; j++) {
            int2 imin = new(-(width >> 1) - (j >> 1), j); // aligned index
            int imax = imin.x + width;
            for (int2 i = imin; i.x < imax; i.x++) {
                Transform cell = CreateCell().transform;
                cell.SetParent(this.transform, false);
                float2 p = HexGrid.GetCellPosition(i);
                cell.localPosition = new Vector3(p.x, p.y, 0f);
                cell.name = string.Format("cell ({0}, {1})", i.x, i.y);
            }
        }
        _isPopulated = true;
    }

    private void RepopulateGrid() {
        for (int i = this.transform.childCount - 1; i >= 0; i--) DestroyImmediate(this.transform.GetChild(i).gameObject);
        PopulateGrid();
    }

    private void PlaceMarker() {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        float2 pos = GetPosition(r), o;
        if (!float.IsNaN(pos.x)) {
            int2 idx = HexGrid.GetCellIndex(pos, out o);
            float2 p = HexGrid.GetCellPosition(idx);
            if (Input.GetMouseButton(0)) p += o;
            if (Input.GetMouseButtonDown(0)) {
                //Debug.Log(String.Format("Index = ({0}, {1}), position = ({2}, {3})", idx.x, idx.y, p.x, p.y));
            }
            if (marker != null) marker.position = this.transform.TransformPoint(p.x, p.y, 0f);
        }
    }

    public void Update() {
        if (!_isPopulated) RepopulateGrid();
        PlaceMarker();
    }

    void OnValidate()  {
        if (Application.isPlaying) {
            //Debug.Log("OnValidate");
            _isPopulated = false;
        }
    }
}
