using UnityEngine;
using Unity.AI.Navigation;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public int xSize = 100;
    public int zSize = 100;

    Color[] colours;
    float minTerrainHeight = 0f; 
    float maxTerrainHeight = 0f;

    public float scale = 0.05f;
    public float heightMultiplier = 8f;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        maxTerrainHeight = heightMultiplier;
        colours = new Color[(xSize + 1) * (zSize + 1)];

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[((xSize +1) * (zSize +1))];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; ++x)
            {
                float y = Mathf.PerlinNoise(x * scale, z * scale) * heightMultiplier;
                vertices[i] = new Vector3(x, y, z);

                float normalizedHeight = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, y);
                colours[i] = Color.Lerp(Color.green, Color.gray, normalizedHeight);
                i++;
            }
        }
            //create triangles
            triangles = new int[xSize * zSize * 6];
            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize; z++)
            {
                for(int x = 0; x < xSize; ++x)
                {
                    //triangle 1
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + xSize + 1;
                    triangles[tris + 2] = vert + 2;

                    //triangle 2
                    triangles[tris + 3] = vert + 0;
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
        mesh.colors = colours; 
        mesh.RecalculateNormals();

        var col = GetComponent<MeshCollider>();
        if (col)
            col.sharedMesh = mesh;

        //implement NavMeshSurface at run time
        var surface = GetComponent<NavMeshSurface>();
        if (surface)
            surface.BuildNavMesh();
    }
}
