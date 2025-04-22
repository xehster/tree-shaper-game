using UnityEngine;

public class KnifeVoxelCutter : MonoBehaviour
{
    public LayerMask voxelLayer;

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & voxelLayer) != 0)
        {
            Destroy(other.gameObject);
        }
    }
}

