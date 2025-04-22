using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BrushVisualizer : MonoBehaviour
{
    public Camera playerCamera;
    public float brushRadius = 0.5f;
    public LayerMask voxelLayer;
    public GameObject brushPrefab;

    public Slider brushSizeSlider;
    public TMP_Text brushSizeLabel;

    public float brushStep = 0.05f;
    public float minBrushSize = 0.05f;  // now tiny!
    public float maxBrushSize = 3f;


    [HideInInspector] public GameObject brushInstance;

    void Start()
    {
        brushInstance = Instantiate(brushPrefab);
        brushInstance.SetActive(false);

        if (brushSizeSlider != null)
        {
            brushSizeSlider.minValue = minBrushSize;
            brushSizeSlider.maxValue = maxBrushSize;
            brushSizeSlider.onValueChanged.AddListener(SetBrushSize);
            SetBrushSize(brushSizeSlider.value);
        }
    }

    void Update()
    {
        // Hotkeys Q/E to change brush size
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float newSize = Mathf.Clamp(brushRadius - brushStep, minBrushSize, maxBrushSize);
            brushSizeSlider.value = newSize;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            float newSize = Mathf.Clamp(brushRadius + brushStep, minBrushSize, maxBrushSize);
            brushSizeSlider.value = newSize;
        }

        // Center screen raycast
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, voxelLayer))
        {
            brushInstance.SetActive(true);
            brushInstance.transform.position = hit.point + hit.normal * 0.01f;
            brushInstance.transform.localScale = Vector3.one * brushRadius * 2f;

            if (Input.GetMouseButton(0))
            {
                TreeChunkCutter.CutVoxels(hit.point, brushRadius);
            }
        }
        else
        {
            brushInstance.SetActive(false);
        }
    }

    void SetBrushSize(float newSize)
    {
        brushRadius = newSize;
        if (brushSizeLabel != null)
            brushSizeLabel.text = $"Size: {brushRadius:F2}";
    }
}
