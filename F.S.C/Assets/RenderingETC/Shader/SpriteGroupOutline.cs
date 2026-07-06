using UnityEngine;
using UnityEngine.UI;

public class SpriteGroupOutline : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera outlineCamera;
    [SerializeField] private RenderTexture outlineTexture;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Shader maskShader;
    [SerializeField] private RawImage outlineRawImage;
    [SerializeField] private Material outlineMaterialTemplate;
    private Material outlineMaterialInstance;

    [Header("Outline")]
    [SerializeField] private Color outlineColor = Color.white;
    [Range(0f, 1f)]
    [SerializeField] private float outlineAlpha = 1f;
    [SerializeField] private float thickness = 0.5f;


    private void Awake()
    {
        ApplyMaterialParams();
        outlineMaterialInstance = new Material(outlineMaterialTemplate);
        outlineRawImage.material = outlineMaterialInstance;
        outlineMaterialInstance.SetColor("_OutlineColor", outlineColor);
    }

    private void LateUpdate()
    {
        SyncCamera();
        ApplyMaterialParams();
    }

    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        ApplyMaterialParams();
    }

    public void SetOutlineAlpha(float alpha)
    {
        outlineAlpha = Mathf.Clamp01(alpha);
        ApplyMaterialParams();
    }

    public void SetThickness(float value)
    {
        thickness = Mathf.Max(0f, value);
        ApplyMaterialParams();
    }

    public void RefreshChildren()
    {

    }


    private void ApplyMaterialParams()
    {
        if (outlineMaterial == null || outlineTexture == null) return;

        outlineMaterial.SetColor("_OutlineColor", outlineColor);
        outlineMaterial.SetFloat("_Alpha", outlineAlpha);
        outlineMaterial.SetFloat("_Thickness", thickness);
        outlineMaterial.SetVector(
            "_TexelSize",
            new Vector4(1f / outlineTexture.width, 1f / outlineTexture.height, 0f, 0f)
        );
    }

    private void SyncCamera()
    {
        if (mainCamera == null || outlineCamera == null) return;

        outlineCamera.transform.position = mainCamera.transform.position;
        outlineCamera.transform.rotation = mainCamera.transform.rotation;
        outlineCamera.orthographic = mainCamera.orthographic;
        outlineCamera.nearClipPlane = mainCamera.nearClipPlane;
        outlineCamera.farClipPlane = mainCamera.farClipPlane;
    }
}