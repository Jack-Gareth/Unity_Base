using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelMechanicsManager))]
public class LevelMechanicsVisualizer : Editor
{
    private const string VISUALIZER_CHILD_NAME = "_EditorVisualization";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelMechanicsManager manager = (LevelMechanicsManager)target;
        UpdateVisualization(manager);
    }

    private void OnSceneGUI()
    {
        LevelMechanicsManager manager = (LevelMechanicsManager)target;
        UpdateVisualization(manager);
    }

    private void UpdateVisualization(LevelMechanicsManager manager)
    {
        if (manager == null) return;

        Transform visualizerTransform = manager.transform.Find(VISUALIZER_CHILD_NAME);
        GameObject visualizerObject;

        if (visualizerTransform == null)
        {
            visualizerObject = new GameObject(VISUALIZER_CHILD_NAME);
            visualizerObject.transform.SetParent(manager.transform);
            visualizerObject.transform.localPosition = Vector3.zero;
            visualizerObject.transform.localRotation = Quaternion.identity;
            visualizerObject.transform.localScale = Vector3.one;
            visualizerObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
        }
        else
        {
            visualizerObject = visualizerTransform.gameObject;
        }

        SpriteRenderer spriteRenderer = visualizerObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = visualizerObject.AddComponent<SpriteRenderer>();
        }

        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = CreateSquareSprite();
        }

        BoxCollider2D collider = manager.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            visualizerObject.transform.localPosition = collider.offset;
            visualizerObject.transform.localScale = new Vector3(collider.size.x, collider.size.y, 1);
        }

        SerializedObject serializedManager = new SerializedObject(manager);
        
        bool blueMechanic = serializedManager.FindProperty("enableBlueMechanic").boolValue;
        bool redMechanic = serializedManager.FindProperty("enableRedMechanic").boolValue;
        bool greenMechanic = serializedManager.FindProperty("enableGreenMechanic").boolValue;
        bool yellowMechanic = serializedManager.FindProperty("enableYellowMechanic").boolValue;

        Color targetColor = serializedManager.FindProperty("disabledColor").colorValue;

        if (blueMechanic)
        {
            targetColor = serializedManager.FindProperty("blueMechanicColor").colorValue;
        }
        else if (redMechanic)
        {
            targetColor = serializedManager.FindProperty("redMechanicColor").colorValue;
        }
        else if (greenMechanic)
        {
            targetColor = serializedManager.FindProperty("greenMechanicColor").colorValue;
        }
        else if (yellowMechanic)
        {
            targetColor = serializedManager.FindProperty("yellowMechanicColor").colorValue;
        }

        spriteRenderer.color = targetColor;
        spriteRenderer.sortingOrder = -10;

        if (!Application.isPlaying)
        {
            spriteRenderer.enabled = true;
        }
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }

    private void OnDisable()
    {
        LevelMechanicsManager manager = (LevelMechanicsManager)target;
        if (manager != null)
        {
            Transform visualizerTransform = manager.transform.Find(VISUALIZER_CHILD_NAME);
            if (visualizerTransform != null)
            {
                DestroyImmediate(visualizerTransform.gameObject);
            }
        }
    }
}
