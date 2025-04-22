using UnityEngine;
using System.Collections.Generic;

public class VoxelTreeGenerator : MonoBehaviour
{
    public static VoxelTreeGenerator Instance { get; private set; }

    [Header("Voxel Settings")]
    public GameObject voxelPrefab;
    public GameObject trunkPrefab;
    public int height = 14;
    public float spacing = 0.35f;
    public float baseRadius = 4f;
    public float jitter = 0.02f;
    [Range(0f, 0.5f)]
    public float voxelSizeJitter = 0.1f;


    [Header("Trunk Settings")]
    public int trunkHeight = 14;
    public int trunkRadius = 1;

    [Header("Foliage Settings")]
    public int foliageStartHeight = 2;

    public static Dictionary<Vector3Int, GameObject> voxelMap = new();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        voxelMap.Clear();

        for (int y = 0; y < height; y++)
        {
            float radiusMultiplier = GetThujaRadiusMultiplier(y);
            float radius = baseRadius * radiusMultiplier;

            for (int x = -(int)radius; x <= (int)radius; x++)
            {
                for (int z = -(int)radius; z <= (int)radius; z++)
                {
                    bool isInsideTreeRadius = x * x + z * z <= radius * radius;
                    bool isTrunk = y < trunkHeight && Mathf.Abs(x) <= trunkRadius && Mathf.Abs(z) <= trunkRadius;
                    bool isFoliage = y >= foliageStartHeight && isInsideTreeRadius;

                    if (isFoliage || isTrunk)
                    {
                        Vector3Int gridPos = new Vector3Int(x, y, z);
                        Vector3 localPos = new Vector3(
                            x * spacing,
                            y * spacing,
                            z * spacing
                        );

                        Vector3 jitterOffset = new Vector3(
                            Random.Range(-jitter, jitter),
                            Random.Range(-jitter, jitter),
                            Random.Range(-jitter, jitter)
                        );

                        GameObject prefabToUse = isTrunk ? trunkPrefab : voxelPrefab;
                        GameObject voxel = Instantiate(prefabToUse, Vector3.zero, Quaternion.identity, transform);
                        voxel.transform.localPosition = localPos + jitterOffset;


                        if (!isTrunk)
                        {
                            float scaleJitter = 0.15f + Random.Range(-voxelSizeJitter, voxelSizeJitter);
                            voxel.transform.localScale = Vector3.one * scaleJitter;
                            voxel.layer = LayerMask.NameToLayer("Voxel");
                            voxel.tag = "Voxel";
                            voxelMap[gridPos] = voxel;
                            
                        }
                        else
                        {
                            voxel.layer = LayerMask.NameToLayer("Default");
                            voxel.tag = "Untagged";
                        }
                    }
                }
            }
        }
    }

    float GetThujaRadiusMultiplier(int y)
    {
        int foliageHeight = height - foliageStartHeight;
        int segment = Mathf.Max(1, foliageHeight / 5); // avoid div by 0
        int foliageY = y - foliageStartHeight;

        if (foliageY < 0) return 0f;

        if (foliageY < segment * 1) return 0.6f;
        if (foliageY < segment * 2) return 0.9f;
        if (foliageY < segment * 3) return 0.6f;
        if (foliageY < segment * 4) return 0.4f;
        return 0.2f; // top
    }

}
