using UnityEngine;

public class SpriteGroupOutlineColor : MonoBehaviour
{
    private static readonly int MaskColorId = Shader.PropertyToID("_MaskColor");

    [SerializeField] private Color outlineColor = Color.white;

    private MaterialPropertyBlock block ;
    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        RefreshRenderers();
        ApplyColor();
    }

    private void OnEnable()
    {
        RefreshRenderers();
        ApplyColor();
    }

    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        ApplyColor();
    }

    public void SetOutlineAlpha(float alpha)
    {
        outlineColor.a = Mathf.Clamp01(alpha);
        ApplyColor();
    }

    public void RefreshRenderers()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void ApplyColor()
    {
        if (spriteRenderers == null) return;

        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer == null) continue;

            spriteRenderer.GetPropertyBlock(block);
            block.SetColor(MaskColorId, outlineColor);
            spriteRenderer.SetPropertyBlock(block);
        }
    }
}