using UnityEngine;
using UnityEditor;

public class CreateMeshAsset
{
#if UNITY_EDITOR	
	
	[MenuItem("Assets/Create/HexGrid cell mesh")]
	public static void CreateAsset() {
		Mesh mesh = HexGrid.cellMesh;
		string fileName = string.Format("Assets/{0}.asset", mesh.name);
		Object obj = AssetDatabase.LoadMainAssetAtPath(fileName);
		if (obj == null) {
    		AssetDatabase.CreateAsset(mesh, fileName);
        } else {
/* 			AssetDatabase.RemoveObjectFromAsset(obj);
			AssetDatabase.AddObjectToAsset(mesh, fileName);
			AssetDatabase.SetMainObject(mesh, fileName);
 */		}
	}
	
#endif
}
