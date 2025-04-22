using UnityEngine;

public class KnifeFollowMouse : MonoBehaviour
{
    public Camera mainCamera;
    public float distanceFromCamera = 2f;

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.origin + ray.direction * distanceFromCamera;
        transform.position = targetPosition;
    }
}