using UnityEngine;

public class ScissorController : MonoBehaviour
{
    [Header("References")]
    public Transform scissors;
    public float moveSpeed = 10f;

    [Header("Snip Tilt")]
    public float snipTiltZ = 25f; 
    public float tiltSpeed = 10f;

    private Vector3 defaultLocalPosition;
    private Quaternion defaultLocalRotation;

    private Transform brushTransform;
    private GameObject brushInstance;

    void Start()
    {
        if (scissors == null)
        {
            Debug.LogError("ScissorController: No scissors assigned!");
            return;
        }

        defaultLocalPosition = scissors.localPosition;
        defaultLocalRotation = scissors.localRotation;

        // Delay lookup for brush
        Invoke(nameof(FindBrush), 0.1f);
    }

    void FindBrush()
    {
        var brushScript = FindObjectOfType<BrushVisualizer>();
        if (brushScript != null && brushScript.brushInstance != null)
        {
            brushInstance = brushScript.brushInstance;
            brushTransform = brushInstance.transform;
        }
        else
        {
            Debug.LogWarning("ScissorController: Brush instance not found.");
        }
    }

    void Update()
    {
        if (scissors == null) return;

        bool validTarget = brushTransform != null && brushInstance.activeSelf && Input.GetMouseButton(0);

        Vector3 targetPosition = validTarget
            ? brushTransform.position
            : scissors.parent.TransformPoint(defaultLocalPosition);

        scissors.position = Vector3.Lerp(scissors.position, targetPosition, Time.deltaTime * moveSpeed);

        Quaternion targetRotation = validTarget
            ? Quaternion.Euler(defaultLocalRotation.eulerAngles + new Vector3(0, 0, snipTiltZ))
            : scissors.parent.rotation * defaultLocalRotation;

        scissors.rotation = Quaternion.Lerp(scissors.rotation, targetRotation, Time.deltaTime * tiltSpeed);
    }
}
