using System.Collections.Generic;
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
        
        List<GameObject> levelParents = new List<GameObject>();
        
        GameObject levelGround = GameObject.Find("Level (Ground)");
        if (levelGround != null)
        {
            levelParents.Add(levelGround);
        }
        
        GameObject levelWall = GameObject.Find("Level (Wall)");
        if (levelWall != null)
        {
            levelParents.Add(levelWall);
        }
       
        
        int setupCount = 0;
        
        foreach (GameObject levelParent in levelParents)
        {
            SpriteRenderer[] renderers = levelParent.GetComponentsInChildren<SpriteRenderer>();
            
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
        }
    }
}