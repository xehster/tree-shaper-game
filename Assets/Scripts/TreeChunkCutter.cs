using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TreeChunkCutter
{
    static Vector3 spacing = new Vector3(0.35f, 0.35f, 0.35f);
    static float neighborCheckRadius = 0.27f; 
    static LayerMask voxelLayer = LayerMask.GetMask("Voxel");
    static LayerMask groundLayer = LayerMask.GetMask("Ground");

    static bool pendingCheck = false;

    public static void CutVoxels(Vector3 center, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, voxelLayer);
        List<GameObject> cutVoxels = new();

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Voxel"))
            {
                GameObject voxel = hit.gameObject;
                voxel.transform.SetParent(null);
                cutVoxels.Add(voxel);
                VoxelTreeGenerator.voxelMap.Remove(GetGridFromVoxel(voxel));
                PrepareVoxelToFall(voxel);
            }
        }

        CheckFloatingVoxels();
    }

    static void CheckFloatingVoxels()
    {
        if (!pendingCheck)
        {
            pendingCheck = true;
            VoxelTreeGenerator.Instance.StartCoroutine(DelayedCheck());
        }
    }

    static IEnumerator DelayedCheck()
{
    yield return new WaitForSeconds(0.05f);
    pendingCheck = false;

    HashSet<GameObject> unvisited = new(VoxelTreeGenerator.voxelMap.Values);
    List<Vector3Int> keysToRemove = new();

    while (unvisited.Count > 0)
    {
        GameObject start = null;

        foreach (var v in unvisited)
        {
            start = v;
            break;
        }

        if (start == null)
            break;

        HashSet<GameObject> cluster = new();
        Queue<GameObject> queue = new();
        queue.Enqueue(start);
        cluster.Add(start);
        unvisited.Remove(start);

        bool connectedToTrunk = false;

        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();

            // 🪵 Check if touching a trunk
            if (Physics.CheckSphere(current.transform.position, neighborCheckRadius, LayerMask.GetMask("Default")))
            {
                Collider[] overlaps = Physics.OverlapSphere(current.transform.position, neighborCheckRadius);
                foreach (var col in overlaps)
                {
                    if (col.gameObject.CompareTag("Untagged") && col.gameObject.name.Contains("Trunk"))
                    {
                        connectedToTrunk = true;
                        break;
                    }
                }
            }

            Collider[] neighbors = Physics.OverlapSphere(current.transform.position, neighborCheckRadius, voxelLayer);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.CompareTag("Voxel"))
                {
                    GameObject nVoxel = neighbor.gameObject;
                    if (!cluster.Contains(nVoxel) && unvisited.Contains(nVoxel))
                    {
                        cluster.Add(nVoxel);
                        queue.Enqueue(nVoxel);
                        unvisited.Remove(nVoxel);
                    }
                }
            }
        }

        if (!connectedToTrunk)
        {
            foreach (var voxel in cluster)
            {
                voxel.transform.SetParent(null);
                PrepareVoxelToFall(voxel);
                keysToRemove.Add(GetGridFromVoxel(voxel));
            }
        }
    }

    foreach (var key in keysToRemove)
    {
        VoxelTreeGenerator.voxelMap.Remove(key);
    }
}




    static bool IsClusterConnectedToGround(GameObject startVoxel, HashSet<GameObject> cluster)
    {
        Queue<GameObject> queue = new();
        queue.Enqueue(startVoxel);
        cluster.Add(startVoxel);

        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();

            if (Physics.CheckSphere(current.transform.position, neighborCheckRadius, groundLayer))
                return true;

            Collider[] neighbors = Physics.OverlapSphere(current.transform.position, neighborCheckRadius, voxelLayer);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.CompareTag("Voxel") && !cluster.Contains(neighbor.gameObject))
                {
                    cluster.Add(neighbor.gameObject);
                    queue.Enqueue(neighbor.gameObject);
                }
            }
        }

        return false;
    }

    static Vector3Int GetGridFromVoxel(GameObject voxel)
    {
        Vector3 local = voxel.transform.localPosition;
        return new Vector3Int(
            Mathf.RoundToInt(local.x / spacing.x),
            Mathf.RoundToInt(local.y / spacing.y),
            Mathf.RoundToInt(local.z / spacing.z)
        );
    }

    static void PrepareVoxelToFall(GameObject voxel)
    {
        if (!voxel.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb = voxel.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;

        if (!voxel.TryGetComponent<Collider>(out _))
        {
            voxel.AddComponent<SphereCollider>();
        }

        VoxelTreeGenerator.Instance.StartCoroutine(FadeAndDestroy(voxel));
    }
    
    static IEnumerator FadeAndDestroy(GameObject voxel, float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);

        if (voxel != null)
        {
            Object.Destroy(voxel);
        }
    }

}
