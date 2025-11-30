using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public int xSize = 100;
    public int zSize = 100;

    public float scale = 0.05f;
    public float heightMultiplier = 8f;
    public float offsetX = 0f;
    public float offsetZ = 0f;

    public int octaves = 4;
    public float persistence = 0.5f;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMesh();
        UpdateMesh();
    }

    void OnValidate()
    {
        xSize = Mathf.Max(1, xSize);
        zSize = Mathf.Max(1, zSize);
        if (Application.isPlaying && mesh != null)
        {
            GenerateMesh();
            UpdateMesh();
        }
    }

    void GenerateMesh()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;

        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float y = 0;

                for (int o = 0; o < octaves; o++)
                {
                    float perlinX = (x + offsetX) * scale * frequency;
                    float perlinZ = (z + offsetZ) * scale * frequency;

                    y += Mathf.PerlinNoise(perlinX, perlinZ) * amplitude;

                    amplitude *= persistence;
                    frequency *= 2f;
                }

                y *= heightMultiplier;

                vertices[i++] = new Vector3(x, y, z);
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var col = GetComponent<MeshCollider>();
        if (col) col.sharedMesh = mesh;
    }
}
