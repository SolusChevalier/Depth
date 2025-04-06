using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class WallMeshGeneratorEditor : EditorWindow
{
    private Texture2D inputTexture;
    private float pixelSize = 1f;
    private string meshName = "WallMesh";

    [MenuItem("Tools/Wall Mesh Generator")]
    public static void ShowWindow()
    {
        GetWindow<WallMeshGeneratorEditor>("Wall Mesh Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Wall Mesh Generator", EditorStyles.boldLabel);
        inputTexture = (Texture2D)EditorGUILayout.ObjectField("Input Texture", inputTexture, typeof(Texture2D), false);
        pixelSize = EditorGUILayout.FloatField("Pixel Size", pixelSize);
        meshName = EditorGUILayout.TextField("Mesh Name", meshName);

        if (GUILayout.Button("Generate Mesh") && inputTexture != null)
        {
            GenerateMeshFromTexture(inputTexture, pixelSize, meshName);
        }
    }

    private void GenerateMeshFromTexture(Texture2D texture, float pixelSize, string name)
    {
        int width = texture.width;
        int height = texture.height;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int quadIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = texture.GetPixel(x, height - 1 - y);

                if (pixel.r > 0.5f && pixel.g > 0.5f && pixel.b > 0.5f)
                {
                    Vector3 origin = new Vector3(x * pixelSize, y * pixelSize, 0);

                    vertices.Add(origin);
                    vertices.Add(origin + new Vector3(pixelSize, 0, 0));
                    vertices.Add(origin + new Vector3(pixelSize, pixelSize, 0));
                    vertices.Add(origin + new Vector3(0, pixelSize, 0));

                    triangles.Add(quadIndex * 4 + 0);
                    triangles.Add(quadIndex * 4 + 2);
                    triangles.Add(quadIndex * 4 + 1);

                    triangles.Add(quadIndex * 4 + 0);
                    triangles.Add(quadIndex * 4 + 3);
                    triangles.Add(quadIndex * 4 + 2);

                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(1, 1));
                    uvs.Add(new Vector2(0, 1));

                    quadIndex++;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        // Create a new GameObject with the mesh
        GameObject obj = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
        obj.GetComponent<MeshFilter>().mesh = mesh;
        obj.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));

        // Optionally save the mesh as an asset
        string path = EditorUtility.SaveFilePanelInProject("Save Mesh", name, "asset", "Save mesh as asset?");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
        }
    }
}