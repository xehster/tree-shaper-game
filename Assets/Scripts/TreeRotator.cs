using UnityEngine;

public class TreeRotator : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        float input = Input.GetAxis("Horizontal"); 
        transform.Rotate(Vector3.up, input * rotationSpeed * Time.deltaTime);
    }
}