using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerChangeColliders : MonoBehaviour
{
    [SerializeField] private float redPhaseDuration = 1f;
    [SerializeField] private Shader worldClipShader;
    [SerializeField][Range(0f, 1f)] private float transparencyAlpha = 0.3f;

    private List<SpriteRenderer> levelRenderers = new List<SpriteRenderer>();
    private List<Material> originalMaterials = new List<Material>();
    private List<Material> instanceMaterials = new List<Material>();
    private List<Collider2D> levelColliders = new List<Collider2D>();
    
    private List<Collider2D> disabledColliders = new List<Collider2D>();
    private List<GameObject> temporarySplitColliders = new List<GameObject>();
    
    private Coroutine redPhaseCoroutine;
    private bool isRedPhaseActive;
    private LevelMechanicsManager currentZone;

    private static readonly string CLIP_MIN_Y = "_ClipMinY";
    private static readonly string CLIP_MAX_Y = "_ClipMaxY";
    private static readonly string CLIP_MIN_X = "_ClipMinX";
    private static readonly string CLIP_MAX_X = "_ClipMaxX";
    private static readonly string USE_CLIPPING = "_UseClipping";
    private static readonly string CLIP_ALPHA = "_ClipAlpha";

    private void Awake()
    {
        if (worldClipShader == null)
        {
            worldClipShader = Shader.Find("Custom/SpriteWorldClip");
        }
        
        FindLevelObjects();
    }

    private void OnEnable()
    {
        PlayerEvents.OnColorAbility += HandleAbilityInput;
    }

    private void OnDisable()
    {
        PlayerEvents.OnColorAbility -= HandleAbilityInput;
        RestoreOriginalMaterials();
        CleanupSplitColliders();
    }

    private void OnDestroy()
    {
        RestoreOriginalMaterials();
        CleanupSplitColliders();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        LevelMechanicsManager zone = other.GetComponent<LevelMechanicsManager>();
        if (zone != null)
        {
            currentZone = zone;
            RefreshLevelObjects();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        LevelMechanicsManager zone = other.GetComponent<LevelMechanicsManager>();
        if (zone != null && zone == currentZone)
        {
            if (redPhaseCoroutine != null)
            {
                StopCoroutine(redPhaseCoroutine);
                redPhaseCoroutine = null;
                EndRedPhase();
            }
            
            currentZone = null;
            RestoreOriginalMaterials();
        }
    }

    private void HandleAbilityInput()
    {
        if (!CanUseRedMechanic()) return;

        RefreshLevelObjects();

        if (redPhaseCoroutine != null)
            StopCoroutine(redPhaseCoroutine);

        redPhaseCoroutine = StartCoroutine(RedPhaseRoutine());
    }

    private bool CanUseRedMechanic()
    {
        bool mechanicEnabled = false;
        bool inZone = false;
        
        if (currentZone != null)
        {
            mechanicEnabled = currentZone.IsRedMechanicEnabled;
            inZone = currentZone.IsPlayerInZone;
        }
        
        return mechanicEnabled && inZone;
    }

    private IEnumerator RedPhaseRoutine()
    {
        isRedPhaseActive = true;

        Bounds zoneBounds = GetZoneBounds();
        ApplyZoneClipping(zoneBounds);
        SplitAndDisableColliders(zoneBounds);

        yield return new WaitForSecondsRealtime(redPhaseDuration);

        EndRedPhase();
        isRedPhaseActive = false;
        redPhaseCoroutine = null;
    }

    private void EndRedPhase()
    {
        RemoveZoneClipping();
        CleanupSplitColliders();
    }

    private void SplitAndDisableColliders(Bounds zoneBounds)
    {
        disabledColliders.Clear();

        foreach (var col in levelColliders)
        {
            if (col == null) continue;

            Bounds colBounds = col.bounds;

            if (IsCompletelyInside(colBounds, zoneBounds))
            {
                col.enabled = false;
                disabledColliders.Add(col);
            }
            else if (zoneBounds.Intersects(colBounds))
            {
                CreateSplitColliders(col, zoneBounds);
                col.enabled = false;
                disabledColliders.Add(col);
            }
        }
    }

    private void CreateSplitColliders(Collider2D originalCollider, Bounds zoneBounds)
    {
        if (originalCollider is BoxCollider2D boxCollider)
        {
            CreateSplitBoxColliders(boxCollider, zoneBounds);
        }
    }

    private void CreateSplitBoxColliders(BoxCollider2D original, Bounds zoneBounds)
    {
        Bounds colBounds = original.bounds;
        
        float colMinX = colBounds.min.x;
        float colMaxX = colBounds.max.x;
        float colMinY = colBounds.min.y;
        float colMaxY = colBounds.max.y;
        
        float zoneMinX = zoneBounds.min.x;
        float zoneMaxX = zoneBounds.max.x;
        float zoneMinY = zoneBounds.min.y;
        float zoneMaxY = zoneBounds.max.y;

        if (colMinX < zoneMinX)
        {
            float width = zoneMinX - colMinX;
            float height = colMaxY - colMinY;
            Vector2 center = new Vector2(colMinX + width * 0.5f, colMinY + height * 0.5f);
            CreateTemporaryBoxCollider(center, new Vector2(width, height), original);
        }

        if (colMaxX > zoneMaxX)
        {
            float width = colMaxX - zoneMaxX;
            float height = colMaxY - colMinY;
            Vector2 center = new Vector2(zoneMaxX + width * 0.5f, colMinY + height * 0.5f);
            CreateTemporaryBoxCollider(center, new Vector2(width, height), original);
        }

        float innerMinX = Mathf.Max(colMinX, zoneMinX);
        float innerMaxX = Mathf.Min(colMaxX, zoneMaxX);

        if (colMinY < zoneMinY)
        {
            float width = innerMaxX - innerMinX;
            float height = zoneMinY - colMinY;
            Vector2 center = new Vector2(innerMinX + width * 0.5f, colMinY + height * 0.5f);
            CreateTemporaryBoxCollider(center, new Vector2(width, height), original);
        }

        if (colMaxY > zoneMaxY)
        {
            float width = innerMaxX - innerMinX;
            float height = colMaxY - zoneMaxY;
            Vector2 center = new Vector2(innerMinX + width * 0.5f, zoneMaxY + height * 0.5f);
            CreateTemporaryBoxCollider(center, new Vector2(width, height), original);
        }
    }

    private void CreateTemporaryBoxCollider(Vector2 worldCenter, Vector2 size, BoxCollider2D original)
    {
        if (size.x <= 0.01f || size.y <= 0.01f) return;

        GameObject tempObj = new GameObject("TempCollider");
        tempObj.layer = original.gameObject.layer;
        tempObj.transform.position = worldCenter;
        
        BoxCollider2D tempCollider = tempObj.AddComponent<BoxCollider2D>();
        tempCollider.size = size;
        tempCollider.isTrigger = original.isTrigger;
        tempCollider.sharedMaterial = original.sharedMaterial;
        tempCollider.offset = Vector2.zero;
        tempCollider.usedByEffector = original.usedByEffector;
        
        if (original.attachedRigidbody != null)
        {
            Rigidbody2D rb = tempObj.AddComponent<Rigidbody2D>();
            rb.bodyType = original.attachedRigidbody.bodyType;
            rb.sharedMaterial = original.attachedRigidbody.sharedMaterial;
            rb.simulated = original.attachedRigidbody.simulated;
            rb.collisionDetectionMode = original.attachedRigidbody.collisionDetectionMode;
        }
        
        temporarySplitColliders.Add(tempObj);
    }

    private void CleanupSplitColliders()
    {
        foreach (var col in disabledColliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }
        disabledColliders.Clear();

        foreach (var obj in temporarySplitColliders)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        temporarySplitColliders.Clear();
    }

    private bool IsCompletelyInside(Bounds objectBounds, Bounds zoneBounds)
    {
        return zoneBounds.Contains(objectBounds.min) && zoneBounds.Contains(objectBounds.max);
    }

    private void ApplyZoneClipping(Bounds zoneBounds)
    {
        int count = Mathf.Min(levelRenderers.Count, instanceMaterials.Count);
        
        for (int i = 0; i < count; i++)
        {
            if (levelRenderers[i] == null || instanceMaterials[i] == null) continue;

            Material mat = instanceMaterials[i];
            
            if (mat.HasProperty(USE_CLIPPING))
            {
                mat.SetFloat(USE_CLIPPING, 1f);
                mat.SetFloat(CLIP_MIN_X, zoneBounds.min.x);
                mat.SetFloat(CLIP_MAX_X, zoneBounds.max.x);
                mat.SetFloat(CLIP_MIN_Y, zoneBounds.min.y);
                mat.SetFloat(CLIP_MAX_Y, zoneBounds.max.y);
                
                if (mat.HasProperty(CLIP_ALPHA))
                {
                    mat.SetFloat(CLIP_ALPHA, transparencyAlpha);
                }
            }
        }
    }

    private void RemoveZoneClipping()
    {
        int count = Mathf.Min(levelRenderers.Count, instanceMaterials.Count);
        
        for (int i = 0; i < count; i++)
        {
            if (levelRenderers[i] == null || instanceMaterials[i] == null) continue;

            Material mat = instanceMaterials[i];
            
            if (mat.HasProperty(USE_CLIPPING))
            {
                mat.SetFloat(USE_CLIPPING, 0f);
            }
        }
    }

    private void FindLevelObjects()
    {
        int ground = LayerMask.NameToLayer("Ground");
        int wall = LayerMask.NameToLayer("Wall");

        var renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        var colliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);

        levelRenderers.Clear();
        originalMaterials.Clear();
        instanceMaterials.Clear();
        levelColliders.Clear();

        Bounds zoneBounds = GetZoneBounds();
        bool hasZoneBounds = currentZone != null;

        foreach (var r in renderers)
        {
            if (r.gameObject.layer == ground || r.gameObject.layer == wall)
            {
                if (!hasZoneBounds || IsWithinZone(r.bounds, zoneBounds))
                {
                    levelRenderers.Add(r);
                    originalMaterials.Add(r.sharedMaterial);
                    
                    Material instanceMat = new Material(worldClipShader);
                    instanceMat.mainTexture = r.sharedMaterial.mainTexture;
                    
                    if (r.sharedMaterial.HasProperty("_Color"))
                    {
                        instanceMat.SetColor("_Color", r.sharedMaterial.GetColor("_Color"));
                    }
                    
                    r.material = instanceMat;
                    instanceMaterials.Add(instanceMat);
                }
            }
        }

        foreach (var c in colliders)
        {
            if (c.gameObject.layer == ground || c.gameObject.layer == wall)
            {
                if (!hasZoneBounds || IsWithinZone(c.bounds, zoneBounds))
                {
                    levelColliders.Add(c);
                }
            }
        }
    }

    private void RefreshLevelObjects()
    {
        RestoreOriginalMaterials();
        FindLevelObjects();
    }

    private void RestoreOriginalMaterials()
    {
        for (int i = 0; i < levelRenderers.Count; i++)
        {
            if (levelRenderers[i] != null && i < originalMaterials.Count && originalMaterials[i] != null)
            {
                levelRenderers[i].sharedMaterial = originalMaterials[i];
            }
        }

        foreach (var mat in instanceMaterials)
        {
            if (mat != null)
                Destroy(mat);
        }

        instanceMaterials.Clear();
    }

    private Bounds GetZoneBounds()
    {
        if (currentZone != null)
        {
            return currentZone.ZoneBounds;
        }
        return new Bounds(Vector3.zero, Vector3.one * 10000f);
    }

    private bool IsWithinZone(Bounds objectBounds, Bounds zoneBounds)
    {
        return zoneBounds.Intersects(objectBounds);
    }
}
