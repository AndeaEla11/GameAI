using UnityEngine;
using System.Collections.Generic;

public class ArtifactSpawner : MonoBehaviour
{
    public Transform terrain;
    public Vector2 areaSize = new Vector2(100, 100);
    public LayerMask groundMask;

    public class ArtifactType
    {
        public string name = "Coin";
        public GameObject prefab;
        [Min(0)] public int count = 3;

        public float minHeight = -999f;
        public float maxHeight = 999f;
        [Range(0f, 70f)] public float maxSlope = 30f;
        [Min(0f)] public float minSpacing = 3f;
    }

    public ArtifactType[] artifacts = new ArtifactType[6];

    public int triesPerItem = 20;

    void Start()
    {
        SpawnAll();
    }

    void SpawnAll()
    {
        var placedPoints = new List<Vector3>();

        foreach (var t in artifacts)
        {
            if (t == null || t.prefab == null) continue;

            int placed = 0;
            int guard = t.count * triesPerItem;

            while (placed < t.count && guard-- > 0)
            {
                if (TryPlaceOne(t, placedPoints))
                    placed++;
            }
        }
    }

    bool TryPlaceOne(ArtifactType t, List<Vector3> all)
    {
        float rx = Random.Range(0f, areaSize.x);
        float rz = Random.Range(0f, areaSize.y);

        Vector3 top = terrain.TransformPoint(new Vector3(rx, 200f, rz));
        if (!Physics.Raycast(top, Vector3.down, out RaycastHit hit, 400f, groundMask))
            return false;

        float slope = Vector3.Angle(hit.normal, Vector3.up);
        if (slope > t.maxSlope) return false;

        if (hit.point.y < t.minHeight || hit.point.y > t.maxHeight)
            return false;

        foreach (var p in all)
        {
            Vector2 a = new Vector2(p.x, p.z);
            Vector2 b = new Vector2(hit.point.x, hit.point.z);
            if ((a - b).sqrMagnitude < t.minSpacing * t.minSpacing)
                return false;
        }

        var go = Instantiate(t.prefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), transform);

        var col = go.GetComponent<Collider>();
        if (col != null) col.isTrigger = false;

        all.Add(hit.point);
        return true;
    }

}
