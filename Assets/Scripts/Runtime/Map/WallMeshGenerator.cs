using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WallMeshGenerator : MonoBehaviour
{
    public Texture2D inputTexture;
    public float pixelSize = 1f;

    private void Start()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        if (inputTexture == null)
        {
            Debug.LogError("No input texture assigned!");
            return;
        }

        Mesh mesh = new Mesh();
        var vertices = new System.Collections.Generic.List<Vector3>();
        var triangles = new System.Collections.Generic.List<int>();
        var uvs = new System.Collections.Generic.List<Vector2>();

        int width = inputTexture.width;
        int height = inputTexture.height;

        int quadIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = inputTexture.GetPixel(x, y);

                // White pixel means wall
                if (pixel.r > 0.5f && pixel.g > 0.5f && pixel.b > 0.5f)
                {
                    // Bottom-left corner of quad
                    Vector3 origin = new Vector3(x * pixelSize, y * pixelSize, 0);

                    // Add 4 vertices (in clockwise order)
                    vertices.Add(origin);
                    vertices.Add(origin + new Vector3(pixelSize, 0, 0));
                    vertices.Add(origin + new Vector3(pixelSize, pixelSize, 0));
                    vertices.Add(origin + new Vector3(0, pixelSize, 0));

                    // Triangle indices
                    triangles.Add(quadIndex * 4 + 0);
                    triangles.Add(quadIndex * 4 + 2);
                    triangles.Add(quadIndex * 4 + 1);

                    triangles.Add(quadIndex * 4 + 0);
                    triangles.Add(quadIndex * 4 + 3);
                    triangles.Add(quadIndex * 4 + 2);

                    // UVs
                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(1, 1));
                    uvs.Add(new Vector2(0, 1));

                    quadIndex++;
                }
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}