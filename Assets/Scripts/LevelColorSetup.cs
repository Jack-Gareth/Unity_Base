using UnityEngine;

public class LevelColorSetup : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Shader spriteColorShader;
    [SerializeField] private bool autoSetupOnStart = true;
    
    private void Awake()
    {
        if (autoSetupOnStart)
        {
            SetupLevelMaterials();
        }
    }
    
    /// <summary>
    /// Sets up all level objects with the sprite color shader
    /// </summary>
    [ContextMenu("Setup Level Materials")]
    public void SetupLevelMaterials()
    {
        if (spriteColorShader == null)
        {
            spriteColorShader = Shader.Find("Custom/SpriteColor");
            if (spriteColorShader == null)
            {
                Debug.LogError("Could not find Custom/SpriteColor shader! Make sure the shader is compiled.");
                return;
            }
        }
        
        // Shader found and will be used
        
        GameObject levelParent = GameObject.Find("Level");
        if (levelParent == null)
        {
            Debug.LogError("Level parent object not found!");
            return;
        }
        
        SpriteRenderer[] renderers = levelParent.GetComponentsInChildren<SpriteRenderer>();
        int setupCount = 0;
        
        foreach (SpriteRenderer renderer in renderers)
        {
            Material newMaterial = new Material(spriteColorShader);
            newMaterial.name = $"LevelColorMaterial_{renderer.name}";
            newMaterial.SetColor("_Color", Color.white);
            
            if (renderer.sprite != null)
            {
                newMaterial.SetTexture("_MainTex", renderer.sprite.texture);
            }
            
            renderer.material = newMaterial;
            setupCount++;
        }
        
        // Level objects setup complete
    }
}