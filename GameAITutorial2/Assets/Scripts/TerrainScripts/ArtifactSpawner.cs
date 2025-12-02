using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArtifactSpawnerSimple : MonoBehaviour
{
    public Transform terrain;
    public Transform player;
    public LayerMask groundMask = ~0;
    public Vector2 areaSize = new Vector2(100f, 100f);

    //NavMesh / debug
    public float navSampleMaxDistance = 4f;
    public bool showPaths = true;
    public Material pathMaterial;

    public GameObject coinPrefab;
    public GameObject healthPrefab;
    public GameObject shieldPrefab;
    public GameObject swordPrefab;
    public GameObject lootPrefab;
    public GameObject trapPrefab;

    public int coinCount = 3, healthCount = 3, shieldCount = 3,
               swordCount = 3, lootCount = 3, trapCount = 3;

    public float minHeight = 0f;
    public float maxHeight = 999f;
    [Range(0f, 70f)] public float maxSlope = 45f;
    public float minSpacing = 2f;

    readonly List<Vector3> placedPoints = new();
    readonly List<LineRenderer> pathLines = new();

    void Start()
    {
        //clear any old debug lines
        for (int i = 0; i < pathLines.Count; i++)
            if (pathLines[i] != null) Destroy(pathLines[i].gameObject);
        pathLines.Clear();
        placedPoints.Clear();

        StartCoroutine(SpawnAfterNavmesh());
    }

    System.Collections.IEnumerator SpawnAfterNavmesh()
    {
        yield return null;
        yield return null;
        SpawnAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            showPaths = !showPaths;
            for (int i = 0; i < pathLines.Count; i++)
                if (pathLines[i] != null) pathLines[i].enabled = showPaths;
        }
    }

    public void SpawnAll()
    {
        GameObject[] prefabs = { coinPrefab, healthPrefab, shieldPrefab, swordPrefab, lootPrefab, trapPrefab };
        int[] counts = { coinCount, healthCount, shieldCount, swordCount, lootCount, trapCount };

        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] == null || counts[i] <= 0) continue;

            for (int n = 0; n < counts[i]; n++)
            {
                int safety = 60;
                while (safety-- > 0 && !TryPlaceOne(prefabs[i])) { }
            }
        }
    }

    bool TryPlaceOne(GameObject prefab)
    {
        float x = Random.Range(0f, areaSize.x);
        float z = Random.Range(0f, areaSize.y);
        Vector3 rayStart = terrain ? terrain.TransformPoint(new Vector3(x, 200f, z))
                                   : new Vector3(x, 200f, z);
        if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 400f, groundMask))
            return false;

        if (hit.point.y < minHeight || hit.point.y > maxHeight) return false;
        float slope = Vector3.Angle(hit.normal, Vector3.up);
        if (slope > maxSlope) return false;

        for (int i = 0; i < placedPoints.Count; i++)
        {
            Vector2 a = new(placedPoints[i].x, placedPoints[i].z);
            Vector2 b = new(hit.point.x, hit.point.z);
            if ((a - b).sqrMagnitude < minSpacing * minSpacing) return false;
        }

        if (player == null) return false;
        if (!NavMesh.SamplePosition(player.position, out NavMeshHit nmStart, navSampleMaxDistance, NavMesh.AllAreas)) return false;
        if (!NavMesh.SamplePosition(hit.point, out NavMeshHit nmGoal, navSampleMaxDistance, NavMesh.AllAreas)) return false;

        var path = new NavMeshPath();
        if (!NavMesh.CalculatePath(nmStart.position, nmGoal.position, NavMesh.AllAreas, path)) return false;
        if (path.status != NavMeshPathStatus.PathComplete) return false;

        Quaternion align = Quaternion.FromToRotation(Vector3.up, hit.normal);
        Instantiate(prefab, hit.point, align);
        if (showPaths) DrawPath(path);

        placedPoints.Add(hit.point);
        return true;
    }

    void DrawPath(NavMeshPath path)
    {
        GameObject go = new("PathLine");
        LineRenderer lr = go.AddComponent<LineRenderer>();

        lr.useWorldSpace = true;
        lr.positionCount = path.corners.Length;
        lr.widthMultiplier = 0.08f;
        lr.numCapVertices = 2;

        if (pathMaterial != null) 
            lr.material = pathMaterial;

        lr.SetPositions(path.corners);
        pathLines.Add(lr);
    }
}